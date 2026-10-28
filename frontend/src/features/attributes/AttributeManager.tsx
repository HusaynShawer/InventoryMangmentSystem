import { useState } from 'react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { FormField } from '@/components/ui/form-field'
import { Dialog } from '@/components/ui/dialog'
import { productAttributesApi } from '@/api/productAttributes'
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import type { ProductAttribute, ProductAttributeInput, ProductAttributeValueInput } from '@/types'

export function AttributeManager() {
  const queryClient = useQueryClient()
  const { data: attributes = [] } = useQuery({
    queryKey: ['product-attributes'],
    queryFn: () => productAttributesApi.list(),
  })

  const [open, setOpen] = useState(false)
  const [name, setName] = useState('')
  const [code, setCode] = useState('')
  const [valueAttrId, setValueAttrId] = useState<string | null>(null)
  const [valueName, setValueName] = useState('')
  const [valueCode, setValueCode] = useState('')

  const createAttr = useMutation({
    mutationFn: (input: ProductAttributeInput) => productAttributesApi.create(input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['product-attributes'] })
      setOpen(false)
      setName('')
      setCode('')
    },
  })

  const addValue = useMutation({
    mutationFn: ({ id, input }: { id: string; input: ProductAttributeValueInput }) => productAttributesApi.addValue(id, input),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['product-attributes'] })
      setValueAttrId(null)
      setValueName('')
      setValueCode('')
    },
  })

  return (
    <div className="space-y-4 p-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Product Attributes</h1>
        <Button onClick={() => setOpen(true)}>Add attribute</Button>
      </div>
      <div className="space-y-3">
        {attributes.map((attr: ProductAttribute) => (
          <div key={attr.id} className="rounded-md border p-3">
            <div className="flex items-center justify-between">
              <div>
                <div className="font-medium">{attr.name}</div>
                <div className="text-xs text-muted-foreground">{attr.code}</div>
              </div>
              <Button size="sm" variant="outline" onClick={() => setValueAttrId(attr.id!)}>
                Add value
              </Button>
            </div>
            <div className="mt-2 flex flex-wrap gap-2">
              {(attr.values || []).map((v: any) => (
                <span key={v.id} className="rounded-full border px-2 py-0.5 text-xs">
                  {v.value} ({v.code})
                </span>
              ))}
            </div>
          </div>
        ))}
      </div>

      <Dialog open={open} onClose={() => setOpen(false)} title="Add attribute">
        <form
          onSubmit={(e) => {
            e.preventDefault()
            createAttr.mutate({ name, code })
          }}
          className="space-y-3"
        >
          <FormField label="Name" required>
            <Input value={name} onChange={(e) => setName(e.target.value)} required />
          </FormField>
          <FormField label="Code" required>
            <Input value={code} onChange={(e) => setCode(e.target.value)} required />
          </FormField>
          <div className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={() => setOpen(false)}>
              Cancel
            </Button>
            <Button type="submit" loading={createAttr.isPending}>
              Create
            </Button>
          </div>
        </form>
      </Dialog>

      <Dialog open={Boolean(valueAttrId)} onClose={() => setValueAttrId(null)} title="Add attribute value">
        <form
          onSubmit={(e) => {
            e.preventDefault()
            if (!valueAttrId) return
            addValue.mutate({ id: valueAttrId, input: { value: valueName, code: valueCode } })
          }}
          className="space-y-3"
        >
          <FormField label="Value" required>
            <Input value={valueName} onChange={(e) => setValueName(e.target.value)} required />
          </FormField>
          <FormField label="Code" required>
            <Input value={valueCode} onChange={(e) => setValueCode(e.target.value)} required />
          </FormField>
          <div className="flex justify-end gap-2">
            <Button type="button" variant="outline" onClick={() => setValueAttrId(null)}>
              Cancel
            </Button>
            <Button type="submit" loading={addValue.isPending}>
              Add
            </Button>
          </div>
        </form>
      </Dialog>
    </div>
  )
}
