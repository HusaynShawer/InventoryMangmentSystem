import { useEffect, useMemo } from 'react'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useQuery } from '@tanstack/react-query'
import { Dialog } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Select } from '@/components/ui/select'
import { FormField } from '@/components/ui/form-field'
import { warehousesApi } from '@/api/warehouses'
import { productHooks } from '@/features/products/hooks'
import { useStockList } from '@/features/stock/hooks'
import { useAuth } from '@/features/auth/useAuth'
import { queryKeys } from '@/lib/constants'
import { formatNumber, shortId } from '@/lib/utils'
import { useAddToCart } from './hooks'

const schema = z.object({
  variantId: z.string().uuid('Select a product variant'),
  warehouseId: z.string().uuid('Select a warehouse'),
  quantity: z.coerce.number().int().positive('Must be at least 1'),
})
type FormValues = z.infer<typeof schema>

interface Props {
  open: boolean
  onClose: () => void
  defaultVariantId?: string
  defaultWarehouseId?: string
}

/** Adds a variant from a specific warehouse, respecting the available reservation. */
export function AddToCartDialog({ open, onClose, defaultVariantId, defaultWarehouseId }: Props) {
  const add = useAddToCart()
  const { hasRole } = useAuth()
  const isAdmin = hasRole('ADMIN')
  const { data: products = [] } = productHooks.useList()
  const { data: stock = [] } = useStockList()
  const { data: warehouses = [] } = useQuery({
    queryKey: queryKeys.warehouses,
    queryFn: warehousesApi.list,
    enabled: isAdmin,
  })

  const variants = useMemo(
    () =>
      products.flatMap((product) =>
        (product.variants ?? []).map((variant) => ({
          ...variant,
          productName: product.name ?? '',
        })),
      ),
    [products],
  )

  const warehouseNames = useMemo(
    () => new Map(warehouses.map((warehouse) => [warehouse.id, warehouse.name ?? 'Warehouse'])),
    [warehouses],
  )

  const availability = useMemo(() => {
    const map = new Map<string, number>()
    for (const row of stock) {
      if (!row.productVariantId || !row.warehouseId) continue
      const quantity = row.availableQuantity ?? row.quantityOnHand ?? row.quantity ?? 0
      map.set(`${row.productVariantId}:${row.warehouseId}`, quantity)
    }
    return map
  }, [stock])

  const {
    register,
    handleSubmit,
    reset,
    watch,
    setValue,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  const variantId = watch('variantId')
  const warehouseId = watch('warehouseId')

  const warehouseOptions = useMemo(
    () =>
      stock
        .filter(
          (row) =>
            row.productVariantId === variantId &&
            row.warehouseId &&
            (availability.get(`${row.productVariantId}:${row.warehouseId}`) ?? 0) > 0,
        )
        .map((row) => ({
          id: row.warehouseId!,
          name: warehouseNames.get(row.warehouseId!) ?? shortId(row.warehouseId),
          available: availability.get(`${row.productVariantId}:${row.warehouseId}`) ?? 0,
        })),
    [stock, variantId, availability, warehouseNames],
  )

  const selectedAvailable = availability.get(`${variantId}:${warehouseId}`) ?? 0

  useEffect(() => {
    if (!open) return
    reset({
      variantId: defaultVariantId ?? '',
      warehouseId: defaultWarehouseId ?? '',
      quantity: 1,
    })
  }, [open, defaultVariantId, defaultWarehouseId, reset])

  useEffect(() => {
    if (!open || !variantId) return
    const valid = warehouseOptions.some((option) => option.id === warehouseId)
    if (!valid) {
      setValue('warehouseId', warehouseOptions[0]?.id ?? '')
    }
  }, [open, variantId, warehouseId, warehouseOptions, setValue])

  const onSubmit = async (values: FormValues) => {
    await add.mutateAsync(values)
    onClose()
  }

  return (
    <Dialog
      open={open}
      onClose={onClose}
      title="Add to cart"
      description="Pick a variant and the warehouse it should be reserved from."
      footer={
        <>
          <Button variant="outline" onClick={onClose} disabled={add.isPending}>
            Cancel
          </Button>
          <Button type="submit" form="add-to-cart-form" loading={add.isPending}>
            Add to cart
          </Button>
        </>
      }
    >
      <form id="add-to-cart-form" onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <FormField label="Variant" htmlFor="variantId" error={errors.variantId?.message} required>
          <Select id="variantId" {...register('variantId')}>
            <option value="">Select a variant…</option>
            {variants.map((variant) => (
              <option key={variant.id} value={variant.id}>
                {variant.productName} — {variant.sku}
              </option>
            ))}
          </Select>
        </FormField>

        <FormField label="Warehouse" htmlFor="warehouseId" error={errors.warehouseId?.message} required>
          <Select id="warehouseId" {...register('warehouseId')} disabled={!variantId}>
            <option value="">{variantId ? 'Select a warehouse…' : 'Select a variant first'}</option>
            {warehouseOptions.map((option) => (
              <option key={option.id} value={option.id}>
                {option.name} ({formatNumber(option.available)} available)
              </option>
            ))}
          </Select>
        </FormField>

        {variantId && warehouseOptions.length === 0 && (
          <p className="text-sm text-destructive">No available stock for this variant.</p>
        )}

        <FormField
          label="Quantity"
          htmlFor="quantity"
          error={errors.quantity?.message}
          hint={warehouseId ? `${formatNumber(selectedAvailable)} available` : undefined}
          required
        >
          <Input
            id="quantity"
            type="number"
            min={1}
            max={Math.max(selectedAvailable, 1)}
            disabled={!warehouseId}
            {...register('quantity', { valueAsNumber: true })}
          />
        </FormField>
      </form>
    </Dialog>
  )
}
