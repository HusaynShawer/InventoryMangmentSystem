# Inventory Management System — Production-Level Code Review

Reviewed: Controllers, Services, Repositories, UnitOfWork, Models, DTOs, Migrations, Program.cs
Method: full code inspection + build + live probing against the local dev database.

---

## Executive Summary

The **architecture shape is right**: Controller → Service → Repository → EF Core with a UnitOfWork, business rules in services, thin controllers, DTOs for Product/Category/Auth, async everywhere, and — importantly — **correct transaction boundaries in Purchase/Sale creation**. That foundation is good and should not be rewritten.

But the **implementation is not production-ready**. It currently has three classes of failure:

1. **Authorization fails open.** Four controllers have no `[Authorize]` at all, `[AllowAnonymous]` on controllers silently overrides `[Authorize]` on actions, and two policies referenced by name don't exist. Verified live: anonymous `POST /api/Auth/register` created an **ADMIN user (HTTP 200)**, anonymous `POST /api/UserWarehouse` inserted a warehouse assignment **(HTTP 201)**, and anonymous `GET /api/Stock` returned data **(HTTP 200)**. The core business rule — "a normal user only accesses warehouses assigned via UserWarehouse" — is **not enforced in any stock-mutating operation**.
2. **A large class of silent no-op writes.** In ~10 service methods, `SaveChangesAsync` is called *before* the mutation, or never called. `DELETE /api/Purchase/{id}` returns 204 and deletes nothing. `PUT /api/Stock/{id}` changes nothing. The API returns success for operations that never happened.
3. **Inventory can be corrupted** through missing `Stock(ProductId, WarehouseId)` uniqueness, no quantity validation (negative quantities *invert* stock), read-modify-write races, and standalone PurchaseItem/SaleItem/StockMovement endpoints that mutate the ledger without touching Stock.

There are also two confirmed functional bugs: all product responses return `stocks: []` (no `Include`), and low-stock ignores `ReorderLevel` in favor of a hardcoded `5`.

Fixing items 1–3 plus the secrets-in-git problem is the entire critical path. No new libraries or patterns are needed.

---

## Architecture Diagram

```
Client
  │
  ▼
[Authorization]  ← ASP.NET Core middleware: JWT auth + role policy (endpoint level)
  │                 + FALLBACK POLICY (currently missing → fail-open)
  │                 + warehouse-access check (currently missing → belongs in services)
  ▼
Controller        ← HTTP concerns only: status codes, DTO binding, claims extraction
  │                 (currently: OK, except a few `throw new Exception` and entity binding)
  ▼
Service           ← Business rules + VALIDATION (DTO annotations → service rules) + TRANSACTION BOUNDARY
  │                 (correctly placed today for Purchase/Sale create; missing elsewhere)
  ▼
Repository        ← Data access only, no SaveChanges, no business rules
  │                 (currently inconsistent: UserRepository saves, BaseRepository doesn't)
  ▼
EF Core           ← UnitOfWork = single SaveChanges / Begin-Commit-Rollback per use case
  │
  ▼
SQL Server        ← Constraints as last line of defense:
                      UNIQUE(ProductId,WarehouseId), CHECK(Quantity >= 0), FKs (Restrict where history matters)

Exception handling: ONE global middleware (ProblemDetails) — replaces try/catch in every controller
```

Where each concern belongs in this architecture:

| Concern | Correct location | Current state |
|---|---|---|
| Authentication | Middleware (JWT) | ✅ Correct |
| Role authorization | Endpoint `[Authorize(Policy)]` + fallback policy | ⚠️ Inconsistent, some missing, 2 policies don't exist |
| Warehouse access | **Service layer**, one shared helper called by every mutating service | ❌ Not implemented anywhere |
| Validation | DTO annotations (auto) + service business rules + DB constraints | ❌ Absent |
| Transactions | Service, one per use case | ✅ Correct in Purchase/Sale Create, missing elsewhere |
| Exception mapping | Global middleware → ProblemDetails | ❌ Absent; raw `Exception` → 500 |

---

## Critical Problems

### C1 — Anonymous registration creates ADMIN accounts 🔴

- **Problem:** `AuthController` has `[AllowAnonymous]` at class level (`Controllers/AuthController.cs:10`). The build itself warns `ASP0026: This [Authorize] attribute is overridden by an [AllowAnonymous] attribute from farther away`. The action-level `AdminOnly` on `register` (line 22) is dead. `RegisterDto.Role` is client-controlled (`Services/Auth/RegisterDto.cs:21`).
- **Why it matters:** Anyone can POST `{"role":"ADMIN"}` and get full admin. Verified live against the dev DB.
- **Fix:** Remove class-level `[AllowAnonymous]`; put `[AllowAnonymous]` only on `login`. Never accept `Role` from the client on register — an admin-only `AssignRole` path, or hardcode `"User"` for self-registration.
- **Priority:** P0

### C2 — Four controllers have zero authorization (fail-open) 🔴

- **Problem:** `UserWarehouseController`, `PurchaseItemsController`, `SaleItemController`, `SupplierController` have no `[Authorize]`. There is no fallback policy in `Program.cs`.
- **Why it matters:** Verified: anonymous `POST /api/UserWarehouse` → **201 persisted**. A user can assign *themselves to any warehouse*, which by itself voids the warehouse-ACL model. `PurchaseItems`/`SaleItem` anonymous writes reached the DB (failed only on a FK constraint, not on auth) — anonymous clients can corrupt purchase/sale documents while bypassing all stock logic.
- **Location:** `Controllers/UserWarehouseController.cs:11`, `PurchaseItemsController.cs:9`, `SaleItemController.cs:9`, `SupplierController.cs:9`
- **Fix:**
  ```csharp
  builder.Services.AddAuthorization(o =>
      o.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
  ```
  so missing attributes fail **closed**, then put explicit `[Authorize(Policy="AdminOnly")]` where required. This one line would have prevented every hole in C2 and C3.
- **Priority:** P0

### C3 — `[AllowAnonymous]` on GET endpoints leaks data 🔴

- **Problem:** `GET /api/Stock` (and `POST /api/Stock`) are anonymous (`Controllers/StockController.cs:21,36`); `GET /api/Product` and `GET /api/Product/{id}` are anonymous (`Controllers/ProductController.cs:22,30`). Verified `GET /api/Stock` → 200 with full data.
- **Why it matters:** `POST /api/Stock` anonymous lets anyone insert arbitrary stock rows — including **duplicates** for the same product/warehouse (see C7).
- **Fix:** Remove these `AllowAnonymous`s (public product readability is not a stated requirement; if it is, make it deliberate on a *public DTO projection* only).
- **Priority:** P0

### C4 — Two authorization policies don't exist → all requests 500 🔴

- **Problem:** `CategoryController` uses `Policy="AminOnly"` (typo, `Controllers/CategoryController.cs:11`); `StockController.Update` uses `Policy="ManagerOrAdmin"` (`Controllers/StockController.cs:48`), never registered in `Program.cs:65-70`.
- **Why it matters:** Verified: `GET /api/Category` → **500**, `PUT /api/Stock/1` → **500**, even for unauthenticated callers (policy resolution throws before the 401 challenge). The Category API and Stock update are completely broken.
- **Fix:** Fix the typo; register `ManagerOrAdmin` or use `AdminOnly`. Add an integration test asserting every referenced policy exists.
- **Priority:** P0

### C5 — Role claim mismatch locks admins out 🔴

- **Problem:** Program.cs:67 requires role `"ADMIN"` while the domain defines `Admin` (`Models/User.cs:34`) and existing tokens/DB rows say `Admin`. .NET role comparison is **case-sensitive** (`IsInRole("ADMIN")` on an `"Admin"` claim returns `false` — verified).
- **Why it matters:** Every existing admin gets 403 on every admin endpoint. Fails closed, but the system is unusable for admins.
- **Fix:** One canonical role value (constant/enum) used by register, token generation, and policies. Validate `Role` against that set — today any string is accepted.
- **Priority:** P0

### C6 — Silent no-op writes: SaveChanges before mutation, or never 🔴

- **Problem:**
  - `GPurchaseService.Delete` — saves, *then* deletes, never saves again → **DELETE returns 204, purchase stays** (`Services/Purchase/GPurchaseItemService.cs:116-121`). Same pattern in `GPurchaseItemsService.Delete`.
  - `GStockService.Update` — saves *then* updates (`Services/Stock/GStockService.cs:44-50`). Same in `GPurchaseItemsService.Update`.
  - No save at all: `GStockService.Delete`, `GWarehouseService.Update/Delete`, `GsaleService.Update/Delete`, `GStockMovementService.Update/Delete`, `GsaleItemService.Update/Delete`, `UserWarehouseService.Delete`.
- **Why it matters:** The API reports success (204/200) for operations that never happened. Worse than a crash — clients and tests will trust it.
- **Root cause:** Two persistence conventions coexist — `UserRepository`/`UserWarehouseRepository` call `SaveChangesAsync` internally, `BaseRepository` does not — so service authors guess.
- **Fix:** One rule: **repositories never save; every service public method ends with exactly one `SaveAsync` after all mutations** (or is wrapped in an explicit transaction). Delete the save-before-mutate lines.
- **Priority:** P0

### C7 — No unique constraint on Stock(ProductId, WarehouseId) 🔴

- **Problem:** `OnModelCreating` has no unique index for Stock (`Data/ApplicationDBContext.cs`); migration creates only non-unique `IX_Stocks_ProductId`/`IX_Stocks_WarehouseId`. `GStockService.Create` accepts a raw `Stock` entity (and is anonymous — C3).
- **Why it matters:** Two stock rows for the same product+warehouse split the quantity. `StockRepository.GetByProductIdAsync` uses `FirstOrDefault` (`Repositories/Stock/StockRepository.cs:14-22`) → purchases/sales silently update *one of* the duplicates. Same disease already visible in data: duplicate SKUs (`SKU-12345` on products 3002 and 3003) because `Sku` has no unique index either.
- **Fix:** `e.HasIndex(s => new { s.ProductId, s.WarehouseId }).IsUnique()` + dedupe migration; unique `Sku`. Make `POST /api/Stock` admin-only and validated.
- **Priority:** P0

### C8 — Warehouse access rule is not enforced in any business operation 🔴

- **Problem:** Nothing checks `UserWarehouse` before Purchase/Sale/Stock/StockMovement mutations. Worse: `WarehouseId` and `CreatedByUserId` come from the client DTO (`Schemas/PurchaseDTO.cs:5-6`, `Schemas/SaleDTO.cs:6-8`), so even *identity attribution* is spoofable. The only place the ACL is used is two read queries (`Repositories/WarehouseRepo.cs:16-36`) — behind `AdminOnly`, so a normal user can't even call them.
- **Why it matters:** The headline business rule is effectively dead code. The feature list says "user-specific warehouse authorization"; the implementation enforces "nobody can do anything, or anyone can do everything, depending on the endpoint."
- **Fix:** One shared helper (e.g. `IWarehouseAccessChecker.EnsureUserCanAccessAsync(userId, warehouseId)`) called at the top of `GPurchaseService.Create`, `GsaleService.Create`, stock updates, and stock reads. Take `CreatedByUserId` from `ClaimTypes.NameIdentifier`, never from the DTO. Admin bypass = one `if (isAdmin) return;` in that single helper — not scattered `if`s.
- **Priority:** P0

### C9 — Secrets committed to source control 🔴

- **Problem:** `appsettings.json` is tracked by git and contains the `sa` password and the JWT signing key (`appsettings.json:10,13`).
- **Why it matters:** The JWT key in the repo allows anyone with repo access to forge tokens forever, regardless of everything else in this review.
- **Fix:** Move both to user-secrets (dev) / environment variables or a secret store (prod), rotate both values now. Connection strings never belong in git.
- **Priority:** P0

### C10 — Stock mutations are not concurrency-safe 🔴

- **Problem:** Purchase and Sale do read-modify-write on `Stock.Quantity` inside a transaction with no concurrency control (`Services/Purchase/GPurchaseItemService.cs:72-101`, `Services/Sale/GsaleService.cs:66-79`). EF issues `UPDATE Stocks SET Quantity=@new WHERE Id=@id` with no original-value predicate.
- **Why it matters:** The classic scenario: A reads 10, B reads 10, A sells 7 (writes 3), B sells 6 (writes 4) → final **4**, but only 3 remain sellable — or with different numbers, **negative stock**. The sale guard `item.Quantity > _stock.Quantity` (line 75) reads stale data. The transaction does *not* save you: READ COMMITTED doesn't prevent lost updates.
- **Fix:** See Transaction & Concurrency section — atomic conditional `ExecuteUpdateAsync` (simplest, no rowversion needed).
- **Priority:** P0

### C11 — No quantity/price validation → negative stock by request 🔴

- **Problem:** `SaleItemDTO`/`PurchaseItemDto` have no `[Range]`. Service only checks the *upper* bound for sales and never for purchases.
- **Why it matters:** `{"quantity": -10}` on a sale **increases** stock (`Quantity -= -10`); on a purchase it **decreases** it. Zero/negative unit prices are accepted too.
- **Fix:** `[Range(1, int.MaxValue)]` on quantities, `[Range(0, …)]` on prices (auto-validated by `[ApiController]`), plus a service-level guard as defense in depth.
- **Priority:** P0

### C12 — Entities bound and returned directly (mass assignment / over-posting) 🟠

- **Problem:** `Stock`, `Warehouse`, `Supplier`, `Purchase`, `Sale`, `StockMovement`, `PurchaseItems`, `SaleItem`, `UserWarehouse` are accepted as request bodies and returned as responses.
- **Why it matters:** A client can set `Id`, `CreatedByUserId`, `TotalAmount`, `UserId`, `Date` on writes. `PurchaseController.Update` (`Controllers/PurchaseController.cs:46`) lets the client rewrite `TotalAmount` while items stay unchanged. Returning entities couples the DB model to the API forever.
- **Fix:** Request DTO + response DTO per resource — already done correctly for Product and Category; copy that pattern.
- **Priority:** P1

### C13 — Standalone PurchaseItem/SaleItem/StockMovement endpoints bypass stock logic 🟠

- **Problem:** These controllers create/update line items and ledger rows directly, with no stock update and (for two of them) no auth.
- **Why it matters:** Purchase items are writable in two ways — one that updates stock and one that doesn't. The StockMovement ledger diverges from `Stocks` on demand.
- **Fix:** Make line-item and StockMovement endpoints **read-only** (or remove them); writes only through `POST /api/Purchase` and `POST /api/Sale`. Manual adjustments → a dedicated `POST /api/stock/adjust` with a reason that writes Stock **and** a movement atomically.
- **Priority:** P1

### C14 — Sale records MovementType = "Purchase" 🟠

- **Problem:** `Services/Sale/GsaleService.cs:87` writes `"Purchase"` for sale movements.
- **Why it matters:** The audit trail lies — every sale appears as a purchase. Silent, and it will take days to notice in real data.
- **Fix:** `"Sale"` + an enum/constant instead of free strings.
- **Priority:** P1

### C15 — No global exception handling; raw exceptions become 500s 🟠

- **Problem:** No `UseExceptionHandler`. Services throw `new Exception("No Purchases found")` for *empty lists*, `KeyNotFoundException` and plain `Exception` for not-found, and controllers `throw new Exception(...)` directly (`Controllers/PurchaseController.cs:49,51`, `Controllers/SaleController.cs:38,51`). AuthController catches *all* exceptions and returns 400/401 (`Controllers/AuthController.cs:33-36,52-55`) — a DB outage becomes "401 Unauthorized".
- **Why it matters:** Not-found = 500, empty list = 404 or 500, infra failure = 401. Clients cannot programmatically handle anything, and try/catch is duplicated in only 2 of 12 controllers (inconsistent, not global).
- **Fix:** Global middleware + `ProblemDetails` + three small exceptions (`NotFoundException`, `ValidationException`, `ForbiddenException`, optionally `ConflictException`). Services throw; one place maps to status codes. Remove try/catch from controllers.
- **Priority:** P1

### C16 — Cascade deletes destroy inventory history 🟠

- **Problem:** Deleting a Product cascades to `Stocks`, `StockMovements`, `PurchaseItems`, `SaleItems`; deleting a Warehouse cascades to `Stocks` + `StockMovements`; deleting a Category cascades to Products (migration `20261001001041_InitialCreate.cs`, `ReferentialAction.Cascade` confirmed).
- **Why it matters:** One `DELETE /api/Category/{id}` can erase the movement history of every product in it. For an inventory system the ledger is the asset.
- **Fix:** `Restrict` on Product/Warehouse/Category → history tables; block deletion when referenced (or soft-delete). Keep cascade only for `UserWarehouse` (already correct).
- **Priority:** P1

### C17 — Product responses always show `stocks: []` 🟠

- **Problem:** `GProductService.GetAll/GetByID` load via `BaseRepository.GetAll()` with no `Include`, and lazy loading isn't enabled. `MapToDto` reads an empty initialized collection. **Verified live:** every product in `GET /api/Product` returns `"stocks":[]` with zero SQL issued against `Stocks`.
- **Why it matters:** Warehouse-specific product/stock visibility — a listed feature — does not work through the Product API.
- **Fix:** Project explicitly: `Select(p => new ProductResponseDTO { … Stocks = p.Stocks.Select(...) })` so EF generates one SQL query with the join. Also fixes entity exposure (C12) at the same time.
- **Priority:** P1

### C18 — Low-stock ignores `ReorderLevel`, hardcodes 5 🟡

- **Problem:** `Repositories/Stock/StockRepository.cs:27` (`Quantity <= 5`) and `Repositories/WarehouseRepo.cs:30` (`Quantity < 5`) — two *different* thresholds, neither using `Product.ReorderLevel`, which is returned to clients as if it worked.
- **Fix:** One definition: `s.Quantity <= s.Product.ReorderLevel` — in the service layer, not the repository.
- **Priority:** P2

---

## Architecture Review

### What is good (keep it)

- **Controller → Service → Repository → EF Core is the right structure for this project.** It matches the problem size, and business logic (purchase → stock → movement) already lives in services, not controllers. Do **not** move to Clean Architecture/CQRS/MediatR/DDD — nothing here justifies it; the problem is enforcement and correctness, not layering.
- **Transaction boundaries are in the correct place.** `GPurchaseService.Create` and `GsaleService.Create` begin the transaction in the service, commit after the final save, roll back on failure — exactly right. The `try/catch { rollback; throw; }` shape is correct.
- **Controllers are mostly thin.** Claims parsing, `id != dto.Id` checks, status codes — proper controller work. The exceptions they throw (C15) are the only real issue.
- **Services are small** (largest is 133 lines). None are "too large."
- **DTO usage for Product/Category/Auth is correct** — explicit `MapToDto`, entities never leak. That's the pattern to replicate.
- **DI is correct:** all scoped, one DbContext per request, open-generic `BaseRepository<>` registration is fine. No singletons holding state.
- **Async/await everywhere**, no sync-over-async, no `Task.Result`.

### What should change (with reasons)

- **Repositories:** consistent, but two problems. (1) `UserRepository`/`UserWarehouseRepository` call `SaveChangesAsync` while `BaseRepository` doesn't — this inconsistency is the *direct cause* of C6. Standardize on "repositories never save." (2) `StockRepository.IncreaseStockAsync`/`DecreaseStockAsync` contain stock-mutation business logic in a repository *and* ignore `warehouseId` *and* are dead code (zero callers, along with `IStockRepository`, which doesn't even match the implementation's signatures). **Delete them.** Other dead abstractions: `Services/PurchaseItem/IPurchaseItem.cs` (check usage), unused `using System.Security.Claims` in services.
- **UnitOfWork:** thin, but *useful* — it's the transaction API and it works. Keep it, don't expand it. (DbContext is technically the unit of work already; rewriting UoW away would be churn with no benefit. Just make every service use it as the single SaveChanges entry point.)
- **Business rules:** correct layer (services), except the dead repository methods and the hardcoded threshold (C18).
- **Maintainability:** the *structure* scales fine. What doesn't scale is the per-author guessing about save/authorization — fixed by the two global rules in C2 and C6.
- **Transactions:** correct in the two operations that need them. Missing on purchase/sale Update (which can move a warehouse without moving stock) — see Business Logic section for the immutability recommendation.
- **DTOs:** correct where used; extend the Product/Category pattern to the nine entity-bound resources.
- **Entities exposed accidentally:** yes, in 9 controllers — C12.
- **Dependency injection:** correct as-is.

### Unnecessary for this project (do not add)

MediatR, CQRS, DDD aggregates, AutoMapper (hand mapping is 10 lines and clearer), Redis/caching (no measurable hotspot exists), FluentValidation (see Validation), full audit-log framework, repository interfaces for every repository.

---

## Security Review

| Item | Status | Evidence |
|---|---|---|
| JWT config | 🟢 Good | Issuer/audience/lifetime/signing key all validated, `ClockSkew = Zero` (`Program.cs:51-62`) |
| Password hashing | 🟢 Good | BCrypt; generic "Invalid username or password" on both wrong-user and wrong-password (`AuthService.cs:68-74`) — no user enumeration |
| Login | 🟡 | No rate limiting / lockout — add built-in rate limiter on `/api/Auth/login` only |
| Broken authorization (IDOR/BOLA) | 🔴 Critical | C1, C2, C3 — anonymous write paths verified live |
| Warehouse ACL | 🔴 Critical | C8 — not enforced anywhere in mutations |
| Privilege escalation | 🔴 Critical | C1 + client-controlled `Role` |
| Spoofed identity | 🔴 | `CreatedByUserId`/`UserId` from request body |
| Mass assignment | 🟠 | C12 — entity binding on 9 resources |
| Missing policies | 🔴 | C4 — 500s (availability issue, and a sign authz isn't tested) |
| Role mismatch | 🔴 | C5 — case-sensitive, verified |
| Secrets in repo | 🔴 | C9 — `sa` password + JWT key tracked in git |
| SQL injection | 🟢 Good | EF Core parameterized everywhere; no raw SQL in the codebase |
| Sensitive data exposure | 🟠 | Entities serialized wholesale; `User` entity isn't returned (good) but `Purchase`/`Sale` expose all columns |
| Swagger exposure | 🟢 Good | Dev-only (`IsDevelopment`) — keep it that way |
| CORS | 🟡 | Not configured. Fine if client is same-origin; if a separate SPA exists, add an explicit allow-list — **never** `AllowAnyOrigin` with credentials |
| Token revocation | 🟡 | Stateless JWT, 60 min, no refresh — acceptable for this scope. Don't build revocation now |
| HTTPS | 🟡 | `UseHttpsRedirection` should be the **first** middleware (currently after auth); duplicate `UseAuthorization()` at `Program.cs:80` and `:90` — remove line 90 |
| Registration flow | 🟠 | Returns 200 instead of 201; catches all exceptions → 400 (masks DB failures) |

**Authorization placement verdict:** Role checks stay at endpoints. Warehouse checks go in **one service-layer helper** — warehouse membership is per-user runtime data, so a static `IAuthorizationRequirement`/handler would just move the same query into a more complex abstraction. A fallback policy makes the system fail closed. That's the whole design; no custom policy provider needed.

---

## Database / EF Core Review

### Good

- Relationships/FKs modeled explicitly in `OnModelCreating` with deliberate delete behaviors for Purchase/Sale (`Restrict` — correct for financial documents).
- Unique indexes on `UserName`, `Email`, `(UserId, WarehouseId)` — the last one proves composite uniqueness is understood; it just wasn't applied where it matters most (Stock).
- FK indexes exist for all foreign keys (EF default, confirmed in migration).
- Migration is in sync with the model (`has-pending-model-changes` → none).
- `Restrict` on `StockMovements.UserId` prevents deleting a user who has history — good instinct.

### Problems, in order of impact

1. **Missing `UNIQUE(ProductId, WarehouseId)` on Stocks** — C7. The single most important DB constraint in an inventory system.
2. **Missing unique index on `Product.Sku`** — duplicates already in dev data (3002/3003). `Sku` is also nullable while being the natural key — make it required + unique.
3. **Cascade delete fan-out** — C16. Product/Warehouse/Category deletes can erase `StockMovements`. Change to `Restrict`.
4. **No CHECK constraints:** `Stocks.Quantity >= 0`, `PurchaseItems.Quantity > 0`, `SaleItem.Quantity > 0`, `UnitPrice >= 0`. Application validation (C11) is primary; DB constraints are the last line that survives bugs and direct SQL.
5. **Decimal precision is warning-driven:** `TotalAmount`/`UnitPrice` produce EF warning 30000 (no store type configured). The migration happened to generate `decimal(18,2)` — specify `HasPrecision(18,2)` explicitly in `OnModelCreating` so it's intentional, not incidental.
6. **N+1 / missing projection:** no `Include`s anywhere — so rather than N+1 you have the *opposite* bug: related data silently absent (C17). `PurchaseController.GetAll` returns purchases with empty `PurchaseItems`. Fix both by projecting to DTOs in queries (`Select`) instead of returning tracked entities.
7. **Tracking everywhere:** all reads are tracked. For `GetAll` on read endpoints add `.AsNoTracking()` (or use the DTO projection, which implies no tracking). Low urgency at current data sizes.
8. **No pagination:** `GetAll` loads entire tables. Acceptable today; `StockMovements`/`PurchaseItems`/`SaleItems` will be the first tables to hurt. Add `?page=&pageSize=` (max 100) to those three — no generic paging abstraction needed.
9. **Missing composite index for the ledger query pattern:** once you paginate movements, add `IX_StockMovements (WarehouseId, Date DESC)` and/or `(ProductId, WarehouseId, Date)`. Not urgent.
10. **`ReferenceId` has no FK** (polymorphic purchase/sale ref) — right call; document it.

### Migration strategy

Single `InitialCreate`, in sync, applied via migrations — correct. Keep `dotnet ef migrations has-pending-model-changes` in your pre-commit habit (it caught nothing today, but it's the cheapest safety net available).

---

## Business Logic / Inventory Review

### Can the system reach an invalid state? Yes — in five independent ways

1. **Negative stock via sale race** (C10): guard reads stale data; last writer wins; no constraint stops `Quantity` going below zero.
2. **Negative stock via request** (C11): `quantity: -10` on a sale *adds* stock; no CHECK constraint catches it.
3. **Split quantities via duplicate Stock rows** (C7): two rows for (product, warehouse); `FirstOrDefault` picks one arbitrarily; the other becomes an invisible ghost balance.
4. **Ledger divergence** (C13): `PurchaseItems`, `SaleItems`, `StockMovement` are directly writable; Stock and movements stop matching documents. Sale movements already record the wrong `MovementType` (C14).
5. **Document edits/deletes without stock reconciliation:**
   - `GPurchaseService.Update` can move a purchase to a **different warehouse** without moving a single unit of stock (`Services/Purchase/GPurchaseItemService.cs:122-132`).
   - If the save bugs (C6) are fixed as-is, `DELETE /api/Purchase/{id}` and `DELETE /api/Sale/{id}` will remove documents while stock stays decremented — permanent drift.
   - **Recommendation:** purchases and sales are *documents*; make them immutable after creation (POST-only, plus `POST /api/purchase/{id}/cancel` that reverses stock with a compensating movement). Simpler and more correct than letting `Update` recompute everything. Deleting posted documents should be forbidden (`Restrict` + 409).

### What is done correctly

- Purchase flow ordering: insert document → get ID → update stock → write movement, all inside one transaction. The `ReferenceId = purchase.Id` linkage is right.
- Sale flow checks availability before decrementing (correct logic, just not concurrency-safe).
- `TotalAmount` computed server-side from line items on **create** (not trusted from client) — good instinct. (The Update path breaks this rule.)
- Stock movement is created for every stock change in purchase/sale — the ledger concept exists; it's the *integrity* that's missing.

### Not implemented (state honestly rather than half-building)

Warehouse transfers: no transfer entity exists. Don't add one now — when you do, it's the same pattern: two stock updates + two movements in one transaction.

---

## Transaction & Concurrency Review

### Where transactions start/commit today

| Operation | Multi-table? | Transaction? | Verdict |
|---|---|---|---|
| Purchase Create | Purchase + Items + Stock + Movement | ✅ Yes, correct boundary | **Correct — the best code in the project** |
| Sale Create | Sale + Items + Stock + Movement | ✅ Yes | **Correct**, plus availability check |
| Purchase/Sale Update | Can change warehouse | ❌ None | Needs transaction + stock reconciliation, or forbid |
| Product Create with Stock | Product + Stock | ❌ Two saves, no transaction | One `SaveAsync` suffices (single SaveChanges is atomic) |
| Manual StockMovement create | Movement only | N/A | Must also change Stock or be disallowed (C13) |
| Everything else | 1 table | Save-at-end is enough | Fix ordering (C6) |

**Correct boundary for this architecture:** the **service use case** — from `BeginTransactionAsync` to `CommitTransactionAsync`, with *all* `SaveChanges` inside (never one save inside and another outside). The two big creates already do this exactly. EF's `SaveChangesAsync` is itself transactional for a single call, so single-table operations don't need an explicit transaction — one save at the end of the method is the rule to enforce.

**SaveChanges called too many times:** `AuthService.RegisterAsync` saves twice (repo + UoW, `AuthService.cs:51-52`) — redundant. Purchase create saves twice *inside* the transaction — intentional and correct (identity values needed before writing movements with `ReferenceId`). The real problem is the inverse: saves that never happen (C6).

**Partial updates possible?** Yes — via the missing-save paths (C6) and any exception outside the transactional creates (purchase Update has none).

### Concurrency — where it's actually necessary

- **Stock: YES — the only place it's genuinely required.** Two options, pick one:
  1. **Atomic conditional update (recommended):** stop reading-then-writing. Use `ExecuteUpdateAsync` — sale: `UPDATE Stocks SET Quantity = Quantity - @q WHERE Id = @id AND Quantity >= @q`, check affected rows; if 0 → "insufficient stock" → 409. Purchase: `SET Quantity = Quantity + @q`. Atomic at the database, **no rowversion, no retry loop, no lost updates**, removes a read from the hot path. ~10-line change in `StockRepository`.
  2. **`rowversion` concurrency token:** correct if you keep read-modify-write, but then you must catch `DbUpdateConcurrencyException` and retry the whole use case. More code, same outcome. **Only add rowversion if you later build "edit stock adjustment form" UI flows** where a user reads, edits, and saves.
- **Product / Warehouse: NO.** Lost updates on name/location edits are acceptable at this scale; last-writer-wins is fine. Don't add tokens "because concurrency."
- **Purchase / Sale: NO token — make them immutable instead.** A concurrency token on a document you shouldn't edit solves the wrong problem.
- **Duplicate Stock creation:** solved by the unique index (C7), not by a rowversion.
- **Duplicate UserWarehouse assignment:** already solved by your `(UserId, WarehouseId)` unique index — but the violation currently surfaces as a 500; map `DbUpdateException` on that index → 409.

---

## Testing Strategy

**Current state: zero test projects.** For a system whose core promise is "stock cannot go wrong," this is the largest gap after authorization.

### 1. Unit tests (service layer, in-memory or SQLite provider) — business rules first

- Sale throws when `quantity > stock`; stock decrements correctly on success.
- Purchase increments stock and creates one movement per line, `MovementType = "Purchase"`, correct `ReferenceId`.
- Sale movement says `"Sale"` (would have caught C14).
- Negative/zero quantity rejected (would have caught C11).
- `UnauthorizedWarehouseAccessException` when user lacks `UserWarehouse` row; admin passes (would have caught C8).
- `TotalAmount` = Σ(qty × price) and ignores client-supplied value.
- Low-stock uses `ReorderLevel`, not `5` (would have caught C18).

### 2. Integration tests (real SQL Server — Testcontainers or dev DB)

- Purchase create writes Purchase + Items + Stock + Movements atomically; then throw mid-way → **verify rollback of all four tables** (the UoW's core promise, currently untested).
- **Unique (ProductId, WarehouseId)** → second stock row for same pair throws (C7).
- Update/delete operations **actually persist** (would have caught the entire C6 class — a test asserting `GET` after `DELETE` returns 404 kills ten bugs at once).
- Concurrent sale test: two tasks selling 6 from stock 10 → exactly one succeeds, stock = 4, never negative (would have caught C10). Run it 50 times.
- Delete tests: deleting a category with products is blocked (C16).

### 3. API tests (`WebApplicationFactory`) — the authorization matrix

- Anonymous → every endpoint except `POST /api/Auth/login` returns 401. **Assert the fallback policy.**
- Every endpoint's policy name resolves (would have caught the `AminOnly` typo → 500, C4).
- `USER` token: allowed on own-warehouse endpoints; 403 on admin endpoints; 403 on other users' warehouses.
- `ADMIN` token: full access (would have caught C5 role mismatch).
- Register with `role: ADMIN` as anonymous → 401 (would have caught C1).
- Mass-assignment probe: POST a body containing `Id`/`CreatedByUserId` → they are ignored.

Write tests **after** fixing P0s (they'd fail now), but the authorization-matrix API test is worth writing first — it's a tripwire that stays green and catches regressions.

---

## Production Readiness Score

**36 / 100**

| Area | Score | Justification |
|---|---|---|
| Architecture | 8/10 | Right structure, right layering, correct transactions in core flows. Lost points for dead abstractions and save-convention chaos |
| Security | 2/10 | Verified anonymous admin registration, anonymous warehouse self-assignment, secrets in git. JWT/crypto choices are sound but irrelevant behind an open door |
| Authorization | 1/10 | The flagship rule (UserWarehouse ACL) is enforced in zero mutating operations; 4 unprotected controllers; 2 broken policies |
| Database | 5/10 | Sound modeling, FKs, some unique indexes, migrations in sync — but missing the one unique index that matters, cascade deletes eating history, no CHECK constraints |
| Transactions | 7/10 | The two operations that needed transactions got them, correctly. Everything else has save-order bugs |
| Concurrency | 2/10 | No protection exactly where lost updates corrupt inventory |
| Error handling | 2/10 | No middleware; "not found" → 500; empty list → 404; DB outage → 401 |
| API design | 4/10 | Consistent routes/verbs and DTOs in places — undercut by entity exposure, 404-on-empty, `Ok(dto)` instead of 201, no pagination |
| Correctness | 3/10 | Silent no-op writes, empty `stocks[]`, wrong movement type, wrong low-stock rule |
| Testing | 0/10 | None |
| Logging/Observability | 3/10 | Default console logging only; no structured business events, no health checks, no request correlation |
| Deployment readiness | 3/10 | Secrets in repo, no environment config story, no health endpoint |

**Reading:** the *design* scores ~7/10, the *implementation* ~3/10. That's the good news — nothing needs redesigning. 100 is unreachable without tests, logging, and a deployment story; fixing the P0 list alone moves you to ~65.

---

## Recommended Roadmap

Do these **in order**. Do not add any new library, framework, or pattern until step 6.

1. **Rotate and remove secrets** (C9): move `Jwt:Key` + connection string to user-secrets / `MapEnvironmentVariables`, rotate both (the JWT key is burned — it's in git history).
2. **Fail closed** (C2): add a `FallbackPolicy = RequireAuthenticatedUser`; remove every `[AllowAnonymous]` except `POST /api/Auth/login`; remove class-level `[AllowAnonymous]` from `AuthController` (fixes C1); add `[Authorize(Policy="AdminOnly")]` to `UserWarehouse`, `Supplier`; make `PurchaseItems`/`SaleItem` **read-only** (C13).
3. **Fix the authorization config** (C4, C5): fix `AminOnly` typo; register or replace `ManagerOrAdmin`; one canonical role constant used by register, token, and policies; drop `Role` from `RegisterDto`; take `CreatedByUserId` from claims (C8 part 1).
4. **Enforce the warehouse ACL** (C8): one `EnsureUserCanAccessAsync(userId, warehouseId)` helper, called from Purchase/Sale/Stock services; admin bypass lives only inside that helper.
5. **Fix the silent no-op writes** (C6): repositories never save; one `SaveAsync` at the end of every mutating service method. Grep for `SaveAsync` and verify order in all call sites.
6. **Write the authorization-matrix API test** (tripwire) and one rollback integration test — before touching inventory logic, so auth can't regress.
7. **Inventory integrity** (C7, C10, C11): unique `(ProductId, WarehouseId)` + dedupe migration + unique `Sku`; `[Range]` validation on quantities/prices; switch stock changes to atomic conditional `ExecuteUpdateAsync`; add `CHECK (Quantity >= 0)`.
8. **Global exception handling** (C15): one middleware + `ProblemDetails` + `NotFoundException`/`ValidationException`/`ForbiddenException`; delete try/catch from controllers; fix empty-list → `200 []` (not 404/500).
9. **Fix confirmed functional bugs** (C14, C17, C18): sale movement type, product DTO projection (with `Stocks`), `ReorderLevel`-based low-stock. Then make purchase/sale **immutable** (cancel-with-reversal instead of Update/Delete).
10. **Database safety** (C16, C12): `Restrict` deletes on Product/Warehouse/Category; request/response DTOs for the nine entity-bound resources (copy the Product pattern).
11. **Business-rule test suite** (the unit/integration list above, especially the concurrency test).
12. **Then** the upgrades: pagination on movement endpoints, `AsNoTracking` on reads, audit fields (`CreatedBy/UpdatedAt` on documents — `StockMovement` already has `UserId`/`Date`/`ReferenceId`, just keep it truthful), soft delete for **Product and Supplier only** (Warehouse → deactivate, never delete; Category → block when products exist; Purchase/Sale → void, never delete; **StockMovement → never delete at all**), rate limiting on login, health checks, structured logging.

### Explicitly do NOT add now

MediatR, CQRS, DDD, Redis, FluentValidation (DataAnnotations + service rules cover everything until rules become conditional/cross-field — only then consider it), AutoMapper, global query filters, an audit-log table, Serilog/OpenTelemetry (plain `ILogger` with a few business events is enough until there is somewhere to ship them).

---

## Final Verdict

The architecture is *good* and the transaction handling in the core flows is *correct* — don't let anyone tell you to rewrite it. The authorization implementation is *bad* (it fails open, and the flagship warehouse rule is unenforced), and the write-path save discipline is *broken* in a way that makes the API lie about success. Fix those two things and the project goes from "portfolio demo" to "something I'd trust in a code review for a junior hire."
