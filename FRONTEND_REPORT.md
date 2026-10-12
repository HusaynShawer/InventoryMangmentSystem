# Final Report

## (a) Endpoint table (discovered)
# (a) API endpoints discovered

| Method | Route | Auth | Params | Request | Responses |
|---|---|---|---|---|---|
| POST | /api/Auth/login | open | - | body | 200 |
| POST | /api/Auth/register | open | - | body | 200 |
| GET | /api/Category | open | - | - | 200 |
| POST | /api/Category | open | - | body | 200 |
| GET | /api/Category/{id} | open | id(path) | - | 200 |
| PUT | /api/Category/{id} | open | id(path) | body | 200 |
| DELETE | /api/Category/{id} | open | id(path) | - | 200 |
| GET | /api/Product | open | - | - | 200 |
| POST | /api/Product | open | - | body | 200 |
| GET | /api/Product/{id} | open | id(path) | - | 200 |
| PUT | /api/Product/{id} | open | id(path) | body | 200 |
| DELETE | /api/Product/{id} | open | id(path) | - | 200 |
| GET | /api/Purchase | open | - | - | 200 |
| POST | /api/Purchase | open | - | body | 200 |
| GET | /api/Purchase/{id} | open | id(path) | - | 200 |
| PUT | /api/Purchase/{id} | open | id(path) | body | 200 |
| DELETE | /api/Purchase/{id} | open | id(path) | - | 200 |
| GET | /api/PurchaseItems | open | - | - | 200 |
| POST | /api/PurchaseItems | open | - | body | 200 |
| GET | /api/PurchaseItems/{id} | open | id(path) | - | 200 |
| PUT | /api/PurchaseItems/{id} | open | id(path) | body | 200 |
| DELETE | /api/PurchaseItems/{id} | open | id(path) | - | 200 |
| GET | /api/Sale | open | - | - | 200 |
| POST | /api/Sale | open | - | body | 200 |
| GET | /api/Sale/{id} | open | id(path) | - | 200 |
| PUT | /api/Sale/{id} | open | id(path) | body | 200 |
| DELETE | /api/Sale/{id} | open | id(path) | - | 200 |
| GET | /api/SaleItem | open | - | - | 200 |
| POST | /api/SaleItem | open | - | body | 200 |
| GET | /api/SaleItem/{id} | open | id(path) | - | 200 |
| PUT | /api/SaleItem/{id} | open | id(path) | body | 200 |
| DELETE | /api/SaleItem/{id} | open | id(path) | - | 200 |
| GET | /api/Stock | open | - | - | 200 |
| POST | /api/Stock | open | - | body | 200 |
| POST | /api/Stock/Transfer | open | - | body | 200 |
| PUT | /api/Stock/adjust | open | - | body | 200 |
| GET | /api/Stock/lowstock | open | warehouseId(query) | - | 200 |
| GET | /api/Stock/{id} | open | id(path) | - | 200 |
| PUT | /api/Stock/{id} | open | id(path) | body | 200 |
| DELETE | /api/Stock/{id} | open | id(path) | - | 200 |
| GET | /api/StockMovement | open | - | - | 200 |
| POST | /api/StockMovement | open | - | body | 200 |
| GET | /api/StockMovement/{id} | open | id(path) | - | 200 |
| PUT | /api/StockMovement/{id} | open | id(path) | body | 200 |
| DELETE | /api/StockMovement/{id} | open | id(path) | - | 200 |
| GET | /api/Supplier | open | - | - | 200 |
| POST | /api/Supplier | open | - | body | 200 |
| GET | /api/Supplier/{id} | open | id(path) | - | 200 |
| PUT | /api/Supplier/{id} | open | id(path) | body | 200 |
| DELETE | /api/Supplier/{id} | open | id(path) | - | 200 |
| GET | /api/UserWarehouse | open | - | - | 200 |
| POST | /api/UserWarehouse | open | - | body | 200 |
| GET | /api/UserWarehouse/{id} | open | id(path) | - | 200 |
| GET | /api/Warehouse | open | - | - | 200 |
| POST | /api/Warehouse | open | - | body | 200 |
| GET | /api/Warehouse/products | open | - | - | 200 |
| GET | /api/Warehouse/products/low | open | - | - | 200 |
| GET | /api/Warehouse/{id} | open | id(path) | - | 200 |
| PUT | /api/Warehouse/{id} | open | id(path) | body | 200 |
| DELETE | /api/Warehouse/{id} | open | id(path) | - | 200 |
---

## (b) Folder tree created
frontend/
  .dockerignore, .env.example, .gitignore
  Dockerfile, index.html, nginx.conf, package.json, tsconfig.json, vite.config.ts
  src/
    api/ (auth, categories, client, products, purchases, sales, stock, stockMovements, suppliers, userWarehouses, warehouses)
    components/
      data/ (DataTable, LineItemsEditor, PageHeader, Pagination, SearchInput, States)
      layout/ (AppLayout, Sidebar, ThemeToggle)
      ui/ (badge, button, card, confirm-dialog, dialog, form-field, input, label, select, skeleton, spinner, table, textarea)
    features/
      auth/ (AuthContext, LoginPage, ProtectedRoute, RoleGuard, useAuth)
      categories/ (hooks, CategoryListPage, CategoryFormDialog)
      dashboard/ (DashboardPage, useDashboard)
      products/ (hooks, ProductListPage, ProductFormDialog)
      suppliers/ (hooks, SupplierListPage, SupplierFormDialog)
      warehouses/ (hooks, WarehouseListPage, WarehouseFormDialog)
      stock/ (hooks, StockListPage, StockFormDialog, AdjustStockDialog, TransferStockDialog, StockMovementListPage, StockMovementFormDialog)
      purchases/ (hooks, PurchaseListPage, PurchaseCreatePage)
      sales/ (hooks, SaleListPage, SaleCreatePage)
      users/ (hooks, UserListPage, UserCreateDialog)
    hooks/ (useTheme)
    lib/ (constants, crud, jwt, storage, utils)
    routes/ (nav, paths, router)
    types/ (api.d.ts, index.ts)
    App.tsx, main.tsx, index.css, vite-env.d.ts

## (c) How to run
1. Local dev (with backend on http://localhost:5000):
   - cd frontend && npm install && npm run dev
2. Docker (one command):
   - docker compose up --build
3. Access: http://localhost:3000 (frontend), http://localhost:5000/swagger (API), http://localhost:1434 (SQL)

## (d) Backend issues to fix (do NOT change)
- Auth: no seed user exists; POST /api/Auth/login returns 401 with admin/admin123. Add a seed (e.g. admin) or document credentials. 
- Authorization: Swagger shows routes as "open" in security schema; frontend still gates by role. Check [Authorize] policies are applied consistently (some controllers like PurchaseItems/SaleItem/UserWarehouse have loose/no role policy) — backend should enforce what the UI expects. 
- Product create: when creating a product, dto.stock is dereferenced in service even if not provided in some cases? Backend already guards (checks stock not null) but the API contract is strict; document this. 
- Supplier/Sale update semantics: some updates take full entities; frontend maps to DTOs where available. 
- Warehouse low products filters by hardcoded threshold < 5 in WarehouseRepo (GetWarehouseLowProducts) rather than ReorderLevel — may be intentional but awkward for UI. 
- Sale/StockMovement movement types: Sale service sets MovementType "Purchase" (copy-paste bug) — confusing for audit. 
- GetAll returning exceptions when empty: many services throw "No X found" if empty; the frontend client normalizes 404 in one helper but some endpoints may return 500/exception instead of 200[]. This is inconsistent. 
- Stock lowstock requires warehouseId; also Warehouse/products/low is separate. 
- User listing: no GET /api/User/Users or similar; frontend can only create/register and see assignments.

## (e) Missing/awkward API endpoints for UI
- No user directory (GET /api/Auth/users or Users controller) — needed to manage users properly.
- No summary endpoint (dashboard counts, today's sales/purchases totals, 7-day trend) — frontend computes client-side from full lists.
- No purchase/sale details endpoints returning items with product names (frontend joins products/warehouses; items endpoints exist but extra joins would help).
- Stock/lowstock takes warehouseId query but no "my warehouses low stock" aggregated view; Warehouse/products/low exists but returns products, not stock details.
- Supplier update in controller: passes {id,...} in body (frontend does) but some cases may expect different shape; also SupplierController has no [Authorize] shown but others do — clarify roles.
- PurchaseItems/SaleItem/UserWarehouse are mostly unprotected; if they are meant to be internal, consider scoping or auth.
- Error shapes: many endpoints throw exceptions -> 500 with message string; frontend extracts, but consistent ProblemDetails would help.
