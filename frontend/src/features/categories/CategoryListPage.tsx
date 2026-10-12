import { useState } from 'react'
import { Pencil, Plus, Trash2 } from 'lucide-react'
import { PageHeader } from '@/components/data/PageHeader'
import { DataTable, type Column } from '@/components/data/DataTable'
import { Button } from '@/components/ui/button'
import { ConfirmDialog } from '@/components/ui/confirm-dialog'
import { extractErrorMessage } from '@/api/client'
import { CategoryFormDialog } from './CategoryFormDialog'
import { categoryHooks } from './hooks'
import type { Category } from '@/types'

export function CategoryListPage() {
  const { data = [], isLoading, error, refetch } = categoryHooks.useList()
  const deleteMutation = categoryHooks.useDelete()

  const [formOpen, setFormOpen] = useState(false)
  const [editing, setEditing] = useState<Category | null>(null)
  const [pendingDelete, setPendingDelete] = useState<Category | null>(null)

  const columns: Column<Category>[] = [
    {
      key: 'name',
      header: 'Name',
      cell: (row) => <span className="font-medium">{row.name}</span>,
      sortValue: (row) => row.name ?? '',
    },
    {
      key: 'actions',
      header: '',
      className: 'w-24 text-right',
      cell: (row) => (
        <div className="flex justify-end gap-1">
          <Button
            variant="ghost"
            size="icon"
            onClick={() => {
              setEditing(row)
              setFormOpen(true)
            }}
            aria-label="Edit"
          >
            <Pencil className="h-4 w-4" />
          </Button>
          <Button
            variant="ghost"
            size="icon"
            onClick={() => setPendingDelete(row)}
            aria-label="Delete"
          >
            <Trash2 className="h-4 w-4 text-destructive" />
          </Button>
        </div>
      ),
    },
  ]

  return (
    <>
      <PageHeader
        title="Categories"
        description="Group products into categories."
        actions={
          <Button
            onClick={() => {
              setEditing(null)
              setFormOpen(true)
            }}
          >
            <Plus className="h-4 w-4" />
            New category
          </Button>
        }
      />

      <DataTable
        columns={columns}
        data={data}
        isLoading={isLoading}
        error={error ? extractErrorMessage(error) : null}
        onRetry={refetch}
        getRowId={(row) => row.id ?? row.name ?? ''}
        searchPlaceholder="Search categories…"
        searchFn={(row, q) => (row.name ?? '').toLowerCase().includes(q)}
        emptyTitle="No categories"
        emptyDescription="Create your first category to organise products."
      />

      <CategoryFormDialog open={formOpen} onClose={() => setFormOpen(false)} category={editing} />

      <ConfirmDialog
        open={Boolean(pendingDelete)}
        onClose={() => setPendingDelete(null)}
        onConfirm={async () => {
          if (pendingDelete) await deleteMutation.mutateAsync(pendingDelete.id!)
          setPendingDelete(null)
        }}
        title="Delete category"
        description={`Delete “${pendingDelete?.name}”? This cannot be undone.`}
        confirmLabel="Delete"
        loading={deleteMutation.isPending}
      />
    </>
  )
}
