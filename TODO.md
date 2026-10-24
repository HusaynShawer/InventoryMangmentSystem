# Inventory Management System - TODO & Roadmap

> Last Updated: 2026-10-23  
> Status: **Naming standardization complete** - Project builds with 0 errors (24 warnings). See CODE_REVIEW.md for full analysis.

This document tells you what is already done vs what needs to be implemented. Treat this as your contribution guide - fix critical issues first before extending with new features.

## Quick Status
- **Build:** ✅ Compiles successfully (0 errors)
- **Architecture:** Controller → Service → Repository → EF Core with UnitOfWork
- **Transactions:** Correct in Purchase/Sale creation
- **Naming:** Standardized to PascalCase, removed `G` prefix, fixed typos, `Schemas/` → `DTOs/`
- **Tests:** ❌ None exist

---

## 1. ALREADY EXECUTED (Done)

### Core Architecture
- [x] Clean layering (Controllers/Services/Repositories/Models/DTOs)
- [x] Entity Framework Core with SQL Server
- [x] UnitOfWork pattern with transaction support
- [x] Repository pattern with BaseRepository
- [x] Async/await throughout
- [x] Dependency injection configured properly
- [x] JWT Authentication setup
- [x] Role-based authorization policies (AdminOnly, SupplierOnly, UserOnly)
- [x] Global exception handler middleware
- [x] Auto-migration on startup

### Models & Data
- [x] All core entities (User, Product, Category, Supplier, Warehouse, Stock, Purchase, Sale, StockMovement, etc.)
- [x] Guid-based IDs throughout
- [x] EF Core relationships configured
- [x] UserWarehouse many-to-many for warehouse access
- [x] Initial migration created

### Features Implemented
- [x] **Auth** - Register/Login with JWT tokens
- [x] **Products** - CRUD with category and stock on creation
- [x] **Categories** - CRUD
- [x] **Suppliers** - CRUD
- [x] **Warehouses** - CRUD with automatic UserWarehouse assignment on create
- [x] **Stock** - CRUD, adjust stock, transfer between warehouses, low stock queries
- [x] **Purchases** - Create with transaction (updates stock + creates movements)
- [x] **Sales** - Create with transaction (validates stock, updates stock + creates movements)
- [x] **Stock Movements** - Audit trail
- [x] **UserWarehouse** - Assignment management

### Naming Standardization (Just Completed)
- [x] All namespaces: `InventoryManagementSystem.*`
- [x] All files/classes renamed to standard names
- [x] DTOs moved to `DTOs/` folder with consistent naming
- [x] Services: `ProductService`, `CategoryService`, etc. (no `G` prefix)
- [x] Repositories: `IStockRepository`, `WarehouseRepository` (fixed typos)
- [x] Models: `PurchaseItem` (was `PurchaseItems`)
- [x] Exception handler: `GlobalExceptionHandler`
- [x] DbContext: `ApplicationDbContext`

---

## 2. CRITICAL - MUST FIX (P0) 

These are security/correctness issues that will cause data corruption or security breaches.

### Security (Fail-Open)
- [ ] **C1 - Anonymous admin registration**: `AuthController` has class-level `[AllowAnonymous]` overriding action `[Authorize(Policy="AdminOnly")]`. Remove class-level, only allow anonymous on login. Don't accept `Role` from client.
- [ ] **C2 - Unprotected controllers**: `UserWarehouseController`, `PurchaseItemsController`, `SaleItemController`, `SupplierController` have no `[Authorize]`. Add fallback policy or explicit auth.
- [ ] **C3 - Anonymous data leaks**: `GET/POST /api/Stock` and `GET /api/Product` are `[AllowAnonymous]`. Review and restrict as appropriate.
- [ ] **C4 - Missing/broken policies**: `CategoryController` uses typo `AminOnly` (should be `AdminOnly`), `StockController.Update` uses non-existent `ManagerOrAdmin`. Fix or register them.
- [ ] **C5 - Role case mismatch**: Policies use `"ADMIN"` (uppercase) but model defaults to `"Admin"`/`"User"`. Standardize role casing.
- [ ] **C9 - Secrets in git**: `appsettings.json` has DB password and JWT key committed. Move to user-secrets/env vars, rotate them.

### Data Integrity
- [ ] **C6 - Silent no-op writes**: Several services save before mutation or never save (Delete/Update in Sale, SaleItem, StockMovement, PurchaseItem, UserWarehouse). Repositories should never save; services save once at end.
- [ ] **C7 - Missing unique constraints**: No unique index on `(ProductId, WarehouseId)` in Stocks - causes duplicate rows. No unique index on `Product.Sku`. Add constraints + dedup migration.
- [ ] **C10 - Race conditions**: Purchase/Sale use read-modify-write without concurrency control. Can oversell/undersell. Use atomic `ExecuteUpdateAsync` or rowversion.
- [ ] **C11 - No validation**: Quantities/prices not validated - negative quantities invert stock. Add `[Range]` attributes and service validation.
- [ ] **C16 - Cascade deletes**: Deleting Product/Warehouse/Category cascades to history tables (StockMovements, PurchaseItems, SaleItems). Change to `Restrict`, prevent deletion if referenced.

### Authorization Logic
- [ ] **C8 - Warehouse ACL not enforced**: `UserWarehouse` membership not checked on Purchase/Sale/Stock mutations. `CreatedByUserId` comes from client DTO (spoofable). Extract claim from token, add service-level warehouse access check.

---

## 3. HIGH PRIORITY - SHOULD FIX (P1)

### API Design & Correctness
- [ ] **C12 - Entity over-posting**: Controllers accept/return entities directly (Stock, Supplier, Purchase, Sale, etc.). Use DTOs for all resources.
- [ ] **C13 - Bypass endpoints**: `PurchaseItems`, `SaleItems`, `StockMovement` CRUD endpoints allow direct manipulation bypassing stock logic. Make read-only or remove.
- [ ] **C14 - Wrong movement type**: Sales recorded as `"Purchase"` in StockMovement. Fix to `"Sale"`.
- [ ] **C15 - Inconsistent errors**: No global exception handling in practice (controllers throw raw exceptions). Use custom exceptions + ProblemDetails consistently.
- [ ] **C17 - Empty stocks in product response**: ProductService doesn't include Stocks. Use `.Include()` or projection.
- [ ] **N1 - Product create bugs**: Null reference when creating product without initial stock, stock movement never saved.

### Architecture Cleanup
- [ ] **N2 - Stock transfer bugs**: Null check missing on source stock, debug Console.WriteLine left.
- [ ] **N4 - DTO type mismatch**: `StockMovementDto.MovementType` was int, fixed to string but verify usage.
- [ ] **N5 - Dead code**: `IStockRepository` signatures don't match implementation fully, `IPurchaseItemService` not properly wired, unused methods.
- [ ] **Repository consistency**: `UserRepository`/`UserWarehouseRepository` call `SaveChangesAsync` internally while others don't. Standardize to "repositories never save".

---

## 4. MEDIUM/LOW PRIORITY (P2+)

### Enhancements
- [ ] **C18 - Low stock hardcoded**: Uses hardcoded `5` in WarehouseRepository, should use `Product.ReorderLevel`. Fixed in StockRepository partially.
- [ ] **Pagination**: Add pagination to GetAll endpoints for large datasets
- [ ] **AsNoTracking**: Use for read-only queries
- [ ] **Input validation**: Add FluentValidation or enhance DataAnnotations
- [ ] **Audit fields**: Consider CreatedAt/UpdatedAt on mutable entities
- [ ] **Soft deletes**: For Products/Warehouses (never delete history)

### Testing
- [ ] **Unit tests**: Service layer tests with mocks/in-memory
- [ ] **Integration tests**: WebApplicationFactory tests
- [ ] **Authorization matrix tests**: Critical for security
- [ ] **Concurrency tests**: For stock race conditions

### DevOps/Deployment
- [ ] **Health checks**: Add `/health` endpoint
- [ ] **Structured logging**: Serilog or built-in structured logging
- [ ] **CORS**: Configure if needed for separate frontend
- [ ] **Rate limiting**: On auth endpoints
- [ ] **Docker**: Already has Dockerfile/docker-compose, needs secrets handling

---

## 5. CORE FOR BUILDING APPS ON TOP (Your Goal)

You want this project to be a **solid core/infrastructure** that you can extend when building apps like e-commerce. Here's what makes sense:

### Already Good as Core
- ✅ Transaction-safe operations (Purchase/Sale patterns)
- ✅ Stock management with movements (audit trail)
- ✅ Warehouse multi-tenancy via UserWarehouse
- ✅ Role-based access control foundation
- ✅ Clean service/repo patterns easy to extend

### Add to Make it "Core"
- [ ] **Base service interface**: `IService<TDto, TEntity>` pattern for consistency
- [ ] **Result pattern**: Instead of throwing exceptions for business rules, return `Result<T>` 
- [ ] **Specification pattern**: For complex queries
- [ ] **Validation layer**: Centralized validation
- [ ] **DTO mapping**: Consider AutoMapper or consistent mappers
- [ ] **API versioning**: `/api/v1/...`
- [ ] **Standardized responses**: `ApiResponse<T>` wrapper

### Extension Points for E-Commerce
When building e-commerce on top, you'll likely need:
- **Customers/Orders**: Similar to Sale but with customer entity
- **Cart**: Session/DB based carts
- **Payments**: Integration points
- **Shipping**: Address management
- **Products catalog**: Already have Product/Category
- **Inventory check**: Already have stock validation

The core handles: **Products, Categories, Warehouses, Stock, Purchases (suppliers), Sales, Users/Roles, Auditing via StockMovements**. This is solid foundation.

---

## Immediate Next Steps (Recommended Order)

1. **Fix security holes** (C1-C5, C9) - Can't go to production/open source safely otherwise
2. **Fix save discipline** (C6) - API currently lies about operations
3. **Add constraints & validation** (C7, C11) - Prevent data corruption
4. **Enforce warehouse ACL** (C8) - Core business rule
5. **Fix the new bugs** (N1-N2) - Product create NRE, transfer issues
6. **Add tests** - Especially auth matrix and stock operations
7. **Clean up entity exposure** (C12) - Better API design

---

## Quick Commands

```bash
# Build
dotnet build

# Run migrations
dotnet ef database update

# Run app
dotnet run

# Add migration
dotnet ef migrations add <MigrationName>
```

## Files to Review
- `CODE_REVIEW.md` - Detailed analysis of all issues
- `Program.cs` - DI and middleware configuration
- `Data/ApplicationDbContext.cs` - Entity relationships
- `Services/` - Business logic
