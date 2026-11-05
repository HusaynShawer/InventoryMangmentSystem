using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryManagementSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStockUseStockLevels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE sl
                SET sl.QuantityOnHand = s.Quantity,
                    sl.AvailableQuantity =
                        CASE WHEN s.Quantity - sl.ReservedQuantity < 0
                            THEN 0
                            ELSE s.Quantity - sl.ReservedQuantity
                        END,
                    sl.UpdatedAt = SYSUTCDATETIME()
                FROM [StockLevels] sl
                INNER JOIN [Stocks] s
                    ON sl.ProductVariantId = s.ProductVariantId
                    AND sl.WarehouseId = s.WarehouseId;

                INSERT INTO [StockLevels]
                    ([Id], [ProductVariantId], [WarehouseId], [QuantityOnHand],
                     [ReservedQuantity], [AvailableQuantity], [CreatedAt], [UpdatedAt])
                SELECT NEWID(), s.ProductVariantId, s.WarehouseId, s.Quantity,
                       0, s.Quantity, SYSUTCDATETIME(), SYSUTCDATETIME()
                FROM [Stocks] s
                WHERE NOT EXISTS (
                    SELECT 1 FROM [StockLevels] sl
                    WHERE sl.ProductVariantId = s.ProductVariantId
                      AND sl.WarehouseId = s.WarehouseId);

                DELETE FROM [StockReservations]
                WHERE StockLevelId IN (
                    SELECT sl.Id FROM [StockLevels] sl
                    WHERE NOT EXISTS (
                            SELECT 1 FROM [ProductVariants] pv
                            WHERE pv.Id = sl.ProductVariantId)
                       OR NOT EXISTS (
                            SELECT 1 FROM [Warehouses] w
                            WHERE w.Id = sl.WarehouseId));

                DELETE FROM [StockMovements]
                WHERE StockLevelId IN (
                    SELECT sl.Id FROM [StockLevels] sl
                    WHERE NOT EXISTS (
                            SELECT 1 FROM [ProductVariants] pv
                            WHERE pv.Id = sl.ProductVariantId)
                       OR NOT EXISTS (
                            SELECT 1 FROM [Warehouses] w
                            WHERE w.Id = sl.WarehouseId));

                DELETE FROM [StockLevels]
                WHERE NOT EXISTS (
                        SELECT 1 FROM [ProductVariants] pv
                        WHERE pv.Id = [StockLevels].ProductVariantId)
                   OR NOT EXISTS (
                        SELECT 1 FROM [Warehouses] w
                        WHERE w.Id = [StockLevels].WarehouseId);
            ");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.CreateIndex(
                name: "IX_StockLevels_WarehouseId",
                table: "StockLevels",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockLevels_ProductVariants_ProductVariantId",
                table: "StockLevels",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLevels_Warehouses_WarehouseId",
                table: "StockLevels",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockLevels_ProductVariants_ProductVariantId",
                table: "StockLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLevels_Warehouses_WarehouseId",
                table: "StockLevels");

            migrationBuilder.DropIndex(
                name: "IX_StockLevels_WarehouseId",
                table: "StockLevels");

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductVariantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stocks_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Stocks_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(@"
                INSERT INTO [Stocks]
                    ([Id], [ProductVariantId], [WarehouseId], [Quantity], [ProductId])
                SELECT NEWID(), sl.ProductVariantId, sl.WarehouseId, sl.QuantityOnHand,
                       (SELECT TOP 1 pv.ProductId
                        FROM [ProductVariants] pv
                        WHERE pv.Id = sl.ProductVariantId)
                FROM [StockLevels] sl;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ProductId",
                table: "Stocks",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ProductVariantId_WarehouseId",
                table: "Stocks",
                columns: new[] { "ProductVariantId", "WarehouseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_WarehouseId",
                table: "Stocks",
                column: "WarehouseId");
        }
    }
}
