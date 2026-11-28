using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriTrace.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class _20261128013557_Init : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "orders",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                BuyerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                ShippingAddress = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                ReceiverPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                PaymentDeadlineUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CancelReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IdempotencyKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_orders", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "regions",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false),
                Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_regions", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "violation_reports",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ReporterUserId = table.Column<Guid>(type: "uuid", nullable: false),
                TargetType = table.Column<int>(type: "integer", nullable: false),
                TargetId = table.Column<Guid>(type: "uuid", nullable: false),
                Reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                EvidenceUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                Status = table.Column<int>(type: "integer", nullable: false),
                ResolvedByModeratorId = table.Column<Guid>(type: "uuid", nullable: true),
                ResolutionNote = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ResolvedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_violation_reports", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "provinces",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false),
                RegionId = table.Column<int>(type: "integer", nullable: false),
                Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_provinces", x => x.Id);
                table.ForeignKey(
                    name: "FK_provinces_regions_RegionId",
                    column: x => x.RegionId,
                    principalTable: "regions",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "cooperatives",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                ProvinceId = table.Column<int>(type: "integer", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_cooperatives", x => x.Id);
                table.ForeignKey(
                    name: "FK_cooperatives_provinces_ProvinceId",
                    column: x => x.ProvinceId,
                    principalTable: "provinces",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "agri_products",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CooperativeId = table.Column<Guid>(type: "uuid", nullable: false),
                ProvinceId = table.Column<int>(type: "integer", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                StockQuantity = table.Column<int>(type: "integer", nullable: false),
                NutritionInfoJson = table.Column<string>(type: "jsonb", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                SuspensionReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_agri_products", x => x.Id);
                table.ForeignKey(
                    name: "FK_agri_products_cooperatives_CooperativeId",
                    column: x => x.CooperativeId,
                    principalTable: "cooperatives",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_agri_products_provinces_ProvinceId",
                    column: x => x.ProvinceId,
                    principalTable: "provinces",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "harvest_batches",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                BatchCode = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                HarvestDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                ExpectedYieldKg = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                QrSignature = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_harvest_batches", x => x.Id);
                table.ForeignKey(
                    name: "FK_harvest_batches_agri_products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "agri_products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "order_items",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                ProductName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                Quantity = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_order_items", x => x.Id);
                table.ForeignKey(
                    name: "FK_order_items_agri_products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "agri_products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_order_items_orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "product_certifications",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false),
                GradeOrCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                IssuedBy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                IssuedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsVerified = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_product_certifications", x => x.Id);
                table.ForeignKey(
                    name: "FK_product_certifications_agri_products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "agri_products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "farming_logs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                HarvestBatchId = table.Column<Guid>(type: "uuid", nullable: false),
                ActivityDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ActivityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                InputMaterial = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                PreHarvestIntervalDays = table.Column<int>(type: "integer", nullable: false),
                PhotoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_farming_logs", x => x.Id);
                table.ForeignKey(
                    name: "FK_farming_logs_harvest_batches_HarvestBatchId",
                    column: x => x.HarvestBatchId,
                    principalTable: "harvest_batches",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_agri_products_CooperativeId",
            table: "agri_products",
            column: "CooperativeId");

        migrationBuilder.CreateIndex(
            name: "IX_agri_products_ProvinceId",
            table: "agri_products",
            column: "ProvinceId");

        migrationBuilder.CreateIndex(
            name: "IX_agri_products_Status",
            table: "agri_products",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_cooperatives_OwnerUserId",
            table: "cooperatives",
            column: "OwnerUserId",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_cooperatives_ProvinceId",
            table: "cooperatives",
            column: "ProvinceId");

        migrationBuilder.CreateIndex(
            name: "IX_farming_logs_HarvestBatchId",
            table: "farming_logs",
            column: "HarvestBatchId");

        migrationBuilder.CreateIndex(
            name: "IX_harvest_batches_BatchCode",
            table: "harvest_batches",
            column: "BatchCode",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_harvest_batches_ProductId",
            table: "harvest_batches",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_order_items_OrderId",
            table: "order_items",
            column: "OrderId");

        migrationBuilder.CreateIndex(
            name: "IX_order_items_ProductId",
            table: "order_items",
            column: "ProductId");

        migrationBuilder.CreateIndex(
            name: "IX_orders_BuyerUserId",
            table: "orders",
            column: "BuyerUserId");

        migrationBuilder.CreateIndex(
            name: "IX_orders_IdempotencyKey",
            table: "orders",
            column: "IdempotencyKey",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_orders_Status_PaymentDeadlineUtc",
            table: "orders",
            columns: new[] { "Status", "PaymentDeadlineUtc" });

        migrationBuilder.CreateIndex(
            name: "IX_product_certifications_ProductId_Type",
            table: "product_certifications",
            columns: new[] { "ProductId", "Type" });

        migrationBuilder.CreateIndex(
            name: "IX_provinces_Code",
            table: "provinces",
            column: "Code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_provinces_RegionId",
            table: "provinces",
            column: "RegionId");

        migrationBuilder.CreateIndex(
            name: "IX_regions_Code",
            table: "regions",
            column: "Code",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_violation_reports_Status",
            table: "violation_reports",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_violation_reports_TargetType_TargetId",
            table: "violation_reports",
            columns: new[] { "TargetType", "TargetId" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "farming_logs");

        migrationBuilder.DropTable(
            name: "order_items");

        migrationBuilder.DropTable(
            name: "product_certifications");

        migrationBuilder.DropTable(
            name: "violation_reports");

        migrationBuilder.DropTable(
            name: "harvest_batches");

        migrationBuilder.DropTable(
            name: "orders");

        migrationBuilder.DropTable(
            name: "agri_products");

        migrationBuilder.DropTable(
            name: "cooperatives");

        migrationBuilder.DropTable(
            name: "provinces");

        migrationBuilder.DropTable(
            name: "regions");
    }
}
