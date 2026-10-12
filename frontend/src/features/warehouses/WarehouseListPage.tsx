import { useState } from 'react'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { PageHeader } from '@/components/data/PageHeader'
import { DataTable, type Column } from '@/components/data/DataTable'
import { Button } from '@/components/ui/button'
import { ConfirmDialog } from '@/components/ui/confirm-dialog'
import { extractErrorMessage } from '@/api/client'
import { WarehouseFormDialog } from './WarehouseFormDialog'
import { warehouseHooks } from './hooks'
import type { Warehouse } from '@/types'

export function WarehouseListPage() {
  const { data = [], isLoading, error, refetch } = warehouseHooks.useList()
  const deleteMutation = warehouseHooks.useDelete()

  const [formOpen, setFormOpen] = useState(false)
  const [editing, setEditing] = useState<Warehouse | null>(null)
  const [pendingDelete, setPendingDelete] = useState<Warehouse | null>(null)

  const columns: Column<Warehouse>[] = [
    { key: 'name', header: 'Name', cell: (r) => <span className="font-medium">{r.name}</span>, sortValue: (r) => r.name ?? '' },
    { key: 'location', header: 'Location', cell: (r) => r.location },
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
        title="Warehouses"
        description="Physical locations that hold stock."
        actions={
          <Button onClick={() => { setEditing(null); setFormOpen(true) }}>
            <Plus className="h-4 w-4" />
            New warehouse
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
        searchPlaceholder="Search warehouses…"
        searchFn={(r, q) => `${r.name} ${r.location}`.toLowerCase().includes(q)}
        emptyTitle="No warehouses"
      />

      <WarehouseFormDialog open={formOpen} onClose={() => setFormOpen(false)} warehouse={editing} />

      <ConfirmDialog
        open={Boolean(pendingDelete)}
        onClose={() => setPendingDelete(null)}
        onConfirm={async () => {
          if (pendingDelete?.id) await deleteMutation.mutateAsync(pendingDelete.id)
          setPendingDelete(null)
        }}
        title="Delete warehouse"
        description={`Delete “${pendingDelete?.name}”? This cannot be undone.`}
        confirmLabel="Delete"
        loading={deleteMutation.isPending}
      />
    </>
  )
}
