import { useState } from 'react'
import { Info, Plus } from 'lucide-react'
import { useQuery } from '@tanstack/react-query'
import { PageHeader } from '@/components/data/PageHeader'
import { DataTable, type Column } from '@/components/data/DataTable'
import { Button } from '@/components/ui/button'
import { warehousesApi } from '@/api/warehouses'
import { userWarehousesApi } from '@/api/userWarehouses'
import { queryKeys } from '@/lib/constants'
import { formatDate, shortId } from '@/lib/utils'
import { UserCreateDialog } from './UserCreateDialog'
import type { UserWarehouse } from '@/types'

/**
 * The backend has no "list users" endpoint, so this page can create users and
 * review warehouse assignments, but cannot display the user directory.
 */
export function UserListPage() {
  const [createOpen, setCreateOpen] = useState(false)
  const { data: assignments = [], isLoading } = useQuery({
    queryKey: queryKeys.userWarehouses,
    queryFn: userWarehousesApi.list,
  })
  const { data: warehouses = [] } = useQuery({
    queryKey: queryKeys.warehouses,
    queryFn: warehousesApi.list,
  })
  const warehouseNames = new Map(warehouses.map((w) => [w.id, w.name]))

  const columns: Column<UserWarehouse>[] = [
    { key: 'user', header: 'User', cell: (r) => <span className="font-mono text-xs">{shortId(r.userId)}</span> },
    {
      key: 'warehouse',
      header: 'Warehouse',
      cell: (r) => warehouseNames.get(r.warehouseId ?? '') ?? shortId(r.warehouseId),
    },
    { key: 'access', header: 'Access level', cell: (r) => r.accessLevel },
    {
      key: 'default',
      header: 'Default',
      cell: (r) => (r.isDefault ? 'Yes' : 'No'),
    },
    { key: 'assigned', header: 'Assigned', cell: (r) => formatDate(r.assignedAt) },
  ]

  return (
    <>
      <PageHeader
        title="Users"
        description="Create accounts and review warehouse access."
        actions={
          <Button onClick={() => setCreateOpen(true)}>
            <Plus className="h-4 w-4" />
            Create user
          </Button>
        }
      />

      <div className="mb-4 flex items-start gap-3 rounded-lg border border-border bg-muted/40 p-4 text-sm">
        <Info className="mt-0.5 h-4 w-4 shrink-0 text-primary" />
        <p className="text-muted-foreground">
          The API exposes user creation and warehouse assignments, but no endpoint to list
          users, so a full directory is not shown here.
        </p>
      </div>

      <h2 className="mb-3 text-sm font-semibold">Warehouse assignments</h2>
      <DataTable
        columns={columns}
        data={assignments}
        isLoading={isLoading}
        getRowId={(r) => r.id ?? ''}
        searchPlaceholder="Search assignments…"
        searchFn={(r, q) =>
          `${warehouseNames.get(r.warehouseId ?? '') ?? ''} ${r.accessLevel}`.toLowerCase().includes(q)
        }
        emptyTitle="No assignments"
      />

      <UserCreateDialog open={createOpen} onClose={() => setCreateOpen(false)} />
    </>
  )
}
