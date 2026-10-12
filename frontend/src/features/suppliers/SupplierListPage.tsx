import { useState } from 'react'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { PageHeader } from '@/components/data/PageHeader'
import { DataTable, type Column } from '@/components/data/DataTable'
import { Button } from '@/components/ui/button'
import { ConfirmDialog } from '@/components/ui/confirm-dialog'
import { extractErrorMessage } from '@/api/client'
import { SupplierFormDialog } from './SupplierFormDialog'
import { supplierHooks } from './hooks'
import type { Supplier } from '@/types'

export function SupplierListPage() {
  const { data = [], isLoading, error, refetch } = supplierHooks.useList()
  const deleteMutation = supplierHooks.useDelete()

  const [formOpen, setFormOpen] = useState(false)
  const [editing, setEditing] = useState<Supplier | null>(null)
  const [pendingDelete, setPendingDelete] = useState<Supplier | null>(null)

  const columns: Column<Supplier>[] = [
    { key: 'name', header: 'Name', cell: (r) => <span className="font-medium">{r.name}</span>, sortValue: (r) => r.name ?? '' },
    { key: 'contact', header: 'Contact', cell: (r) => r.contactInfo },
    {
      key: 'actions',
      header: '',
      className: 'w-24 text-right',
      cell: (row) => (
        <div className="flex justify-end gap-1">
          <Button variant="ghost" size="icon" onClick={() => { setEditing(row); setFormOpen(true) }} aria-label="Edit">
            <Pencil className="h-4 w-4" />
          </Button>
          <Button variant="ghost" size="icon" onClick={() => setPendingDelete(row)} aria-label="Delete">
            <Trash2 className="h-4 w-4 text-destructive" />
          </Button>
        </div>
      ),
    },
  ]

  return (
    <>
      <PageHeader
        title="Suppliers"
        description="Vendors you purchase stock from."
        actions={
          <Button onClick={() => { setEditing(null); setFormOpen(true) }}>
            <Plus className="h-4 w-4" />
            New supplier
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={data}
        isLoading={isLoading}
        error={error ? extractErrorMessage(error) : null}
        onRetry={refetch}
        getRowId={(r) => r.id ?? r.name ?? ''}
        searchPlaceholder="Search suppliers…"
        searchFn={(r, q) => `${r.name} ${r.contactInfo}`.toLowerCase().includes(q)}
        emptyTitle="No suppliers"
      />

      <SupplierFormDialog open={formOpen} onClose={() => setFormOpen(false)} supplier={editing} />

      <ConfirmDialog
        open={Boolean(pendingDelete)}
        onClose={() => setPendingDelete(null)}
        onConfirm={async () => {
          if (pendingDelete?.id) await deleteMutation.mutateAsync(pendingDelete.id)
          setPendingDelete(null)
        }}
        title="Delete supplier"
        description={`Delete “${pendingDelete?.name}”? This cannot be undone.`}
        confirmLabel="Delete"
        loading={deleteMutation.isPending}
      />
    </>
  )
}
