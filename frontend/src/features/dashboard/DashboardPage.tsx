import {
  Bar,
  BarChart,
  CartesianGrid,
  Legend,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from 'recharts'
import {
  AlertTriangle,
  Boxes,
  Package,
  ShoppingCart,
  TrendingUp,
} from 'lucide-react'
import type { LucideIcon } from 'lucide-react'
import { PageHeader } from '@/components/data/PageHeader'
import { ChartCard, ErrorState, TableSkeleton } from '@/components/data/States'
import { Card } from '@/components/ui/card'
import { extractErrorMessage } from '@/api/client'
import { useAuth } from '@/features/auth/useAuth'
import { formatCurrency, formatNumber } from '@/lib/utils'
import { useDashboard } from './useDashboard'

function StatCard({
  label,
  value,
  sub,
  icon: Icon,
  tone = 'default',
}: {
  label: string
  value: string
  sub?: string
  icon: LucideIcon
  tone?: 'default' | 'warning' | 'success'
}) {
  const toneClass =
    tone === 'warning'
      ? 'bg-warning/15 text-warning'
      : tone === 'success'
        ? 'bg-success/15 text-success'
        : 'bg-primary/10 text-primary'
  return (
    <Card className="flex items-center gap-4 p-5">
      <div className={`flex h-11 w-11 items-center justify-center rounded-lg ${toneClass}`}>
        <Icon className="h-5 w-5" />
      </div>
      <div className="min-w-0">
        <p className="text-sm text-muted-foreground">{label}</p>
        <p className="text-xl font-semibold">{value}</p>
        {sub && <p className="truncate text-xs text-muted-foreground">{sub}</p>}
      </div>
    </Card>
  )
}

const tooltipStyle = {
  backgroundColor: 'var(--color-card)',
  border: '1px solid var(--color-border)',
  borderRadius: 8,
  color: 'var(--color-card-foreground)',
}

export function DashboardPage() {
  const { hasRole } = useAuth()
  const isAdmin = hasRole('ADMIN')
  const { metrics, stockByWarehouse, isLoading, error, refetch } = useDashboard(isAdmin, isAdmin)

  if (isLoading) {
    return (
      <>
        <PageHeader title="Dashboard" description="Overview of your inventory." />
        <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
          {Array.from({ length: 4 }).map((_, index) => (
            <Card key={index} className="p-5">
              <div className="h-11 w-11 animate-pulse rounded-lg bg-muted" />
            </Card>
          ))}
        </div>
        <div className="mt-6">
          <TableSkeleton rows={4} cols={6} />
        </div>
      </>
    )
  }

  if (error) {
    return (
      <>
        <PageHeader title="Dashboard" />
        <div className="rounded-lg border border-border bg-card">
          <ErrorState message={extractErrorMessage(error)} onRetry={refetch} />
        </div>
      </>
    )
  }

  return (
    <>
      <PageHeader title="Dashboard" description="Overview of your inventory." />

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <StatCard
          label="Products"
          value={formatNumber(metrics.productCount)}
          icon={Package}
        />
        <StatCard
          label="Low stock"
          value={formatNumber(metrics.lowStockCount)}
          sub="At or below reorder level"
          icon={AlertTriangle}
          tone="warning"
        />
        {isAdmin ? (
          <>
            <StatCard
              label="Today's sales"
              value={formatCurrency(metrics.todaysSalesTotal)}
              sub={`${metrics.todaysSalesCount} sale(s)`}
              icon={TrendingUp}
              tone="success"
            />
            <StatCard
              label="Today's purchases"
              value={formatCurrency(metrics.todaysPurchasesTotal)}
              sub={`${metrics.todaysPurchasesCount} purchase(s)`}
              icon={ShoppingCart}
            />
          </>
        ) : (
          <StatCard
            label="Stock records"
            value={formatNumber(metrics.stock.length)}
            icon={Boxes}
          />
        )}
      </div>

      {isAdmin && (
        <div className="mt-6 grid gap-4 lg:grid-cols-2">
          <ChartCard title="Last 7 days" subtitle="Sales vs purchases value">
            <ResponsiveContainer width="100%" height={280}>
              <BarChart data={metrics.trend}>
                <CartesianGrid strokeDasharray="3 3" stroke="var(--color-border)" vertical={false} />
                <XAxis dataKey="label" stroke="var(--color-muted-foreground)" fontSize={12} />
                <YAxis stroke="var(--color-muted-foreground)" fontSize={12} />
                <Tooltip contentStyle={tooltipStyle} />
                <Legend />
                <Bar dataKey="sales" name="Sales" fill="#6366f1" radius={[4, 4, 0, 0]} />
                <Bar dataKey="purchases" name="Purchases" fill="#22c55e" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </ChartCard>

          <ChartCard title="Stock by warehouse" subtitle="Total units on hand">
            <ResponsiveContainer width="100%" height={280}>
              <BarChart data={stockByWarehouse} layout="vertical" margin={{ left: 12 }}>
                <CartesianGrid strokeDasharray="3 3" stroke="var(--color-border)" horizontal={false} />
                <XAxis type="number" stroke="var(--color-muted-foreground)" fontSize={12} />
                <YAxis
                  type="category"
                  dataKey="name"
                  stroke="var(--color-muted-foreground)"
                  fontSize={12}
                  width={90}
                />
                <Tooltip contentStyle={tooltipStyle} />
                <Bar dataKey="quantity" name="Units" fill="#0ea5e9" radius={[0, 4, 4, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </ChartCard>
        </div>
      )}
    </>
  )
}
