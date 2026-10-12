import { useFieldArray, useWatch } from 'react-hook-form'
import { Plus, Trash2 } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Select } from '@/components/ui/select'
import { formatCurrency } from '@/lib/utils'
import type { Product } from '@/types'

export interface LineItemValues {
  productId: string
  quantity: number
  unitPrice: number
}

interface LineItemsEditorProps {
  // react-hook-form's generic types are invariant, so this shared editor keeps
  // them loose while the create pages retain strongly typed form values.
  control: any
  register: any
  setValue: any
  errors: any
  products: Product[]
  unitPriceHint?: string
}

/**
 * Reusable dynamic row editor for purchase/sale line items. Rows live in the
 * parent form under `items`, so the parent stays the single source of truth and
 * can compute totals with `watch('items')`.
 */
export function LineItemsEditor({
  control,
  register,
  setValue,
  errors,
  products,
  unitPriceHint = 'Unit price',
}: LineItemsEditorProps) {
  const { fields, append, remove } = useFieldArray({ control, name: 'items' })

  const addRow = () => append({ productId: '', quantity: 1, unitPrice: 0 })

  return (
    <div className="space-y-3">
      <div className="hidden gap-3 px-1 text-xs font-semibold uppercase tracking-wide text-muted-foreground sm:grid sm:grid-cols-[1fr_100px_130px_110px_40px]">
        <span>Product</span>
        <span>Quantity</span>
        <span>{unitPriceHint}</span>
        <span className="text-right">Line total</span>
        <span />
      </div>

      {fields.map((field, index) => {
        const rowError = errors.items?.[index] as
          | { productId?: { message?: string } }
          | undefined
        return (
          <div
            key={field.id}
            className="grid grid-cols-1 gap-3 rounded-md border border-border p-3 sm:grid-cols-[1fr_100px_130px_110px_40px] sm:items-center sm:border-0 sm:p-0"
          >
            <div>
              <Select
                {...register(`items.${index}.productId` as const)}
                onChange={(event) => {
                  // Default the unit price to the product's catalogue price.
                  const product = products.find((p) => p.id === event.target.value)
                  setValue(`items.${index}.productId`, event.target.value)
                  if (product) setValue(`items.${index}.unitPrice`, product.unitPrice ?? 0)
                }}
              >
                <option value="">Select product…</option>
                {products.map((product) => (
                  <option key={product.id} value={product.id}>
                    {product.name} ({product.sku})
                  </option>
                ))}
              </Select>
              {rowError?.productId && (
                <p className="mt-1 text-xs text-destructive">{rowError.productId.message}</p>
              )}
            </div>

            <Input
              type="number"
              min={1}
              {...register(`items.${index}.quantity` as const, { valueAsNumber: true })}
            />

            <Input
              type="number"
              min={0}
              step="0.01"
              {...register(`items.${index}.unitPrice` as const, { valueAsNumber: true })}
            />

            <LineTotal control={control} index={index} />

            <Button
              type="button"
              variant="ghost"
              size="icon"
              onClick={() => remove(index)}
              disabled={fields.length === 1}
              aria-label="Remove line"
            >
              <Trash2 className="h-4 w-4 text-destructive" />
            </Button>
          </div>
        )
      })}

      {errors.items && !Array.isArray(errors.items) && (
        <p className="text-xs text-destructive">{String(errors.items.message ?? '')}</p>
      )}

      <Button type="button" variant="outline" size="sm" onClick={addRow}>
        <Plus className="h-4 w-4" />
        Add line
      </Button>
    </div>
  )
}

/** Reads the row values from the form to show a live line total. */
function LineTotal({ control, index }: { control: any; index: number }) {
  const [quantity, unitPrice] = useWatch({
    control,
    name: [`items.${index}.quantity`, `items.${index}.unitPrice`],
  })
  return (
    <span className="text-right font-medium sm:text-right">
      {formatCurrency(Number(quantity ?? 0) * Number(unitPrice ?? 0))}
    </span>
  )
}
