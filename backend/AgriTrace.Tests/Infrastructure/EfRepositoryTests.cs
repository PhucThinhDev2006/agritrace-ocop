using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Enums;
using AgriTrace.Infrastructure.Persistence;
using AgriTrace.Infrastructure.Persistence.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
namespace AgriTrace.Tests.Infrastructure;

public class EfRepositoryTests
{
    private static async Task<(SqliteConnection Conn, AgriTraceDbContext Db)> CreateSqliteDbAsync()
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        await conn.OpenAsync();
        var options = new DbContextOptionsBuilder<AgriTraceDbContext>().UseSqlite(conn).Options;
        var db = new AgriTraceDbContext(options);
        await db.Database.EnsureCreatedAsync();
        return (conn, db);
    }
    [Fact]
    public async Task SeedAsync_ShouldPopulateData_AndBeIdempotent()
    {
        var (conn, db) = await CreateSqliteDbAsync();
        await using (conn)
        await using (db)
        {
            // Chạy Seed 2 lần liên tiếp để kiểm tra tính Idempotent (không trùng)
            await AgriTraceDbSeeder.SeedAsync(db);
            await AgriTraceDbSeeder.SeedAsync(db);

            Assert.Equal(3, await db.Regions.CountAsync());
            Assert.Equal(63, await db.Provinces.CountAsync());
            Assert.Equal(9, await db.Cooperatives.CountAsync());
            Assert.Equal(18, await db.AgriProducts.CountAsync());
            Assert.Equal(2, await db.HarvestBatches.CountAsync());
        }
    }
    [Fact]
    public async Task ProductRepository_ShouldFilterByRegionAndCertification()
    {
        var (conn, db) = await CreateSqliteDbAsync();
        await using (conn)
        await using (db)
        {
            await AgriTraceDbSeeder.SeedAsync(db);
            var productRepo = new EfProductRepository(db);
            var regionRepo = new EfRegionRepository(db);
            var northProvinceIds = (await regionRepo.GetProvincesByRegionAsync(1)).Select(p => p.Id).ToHashSet();
            var northOcopProducts = await productRepo.GetApprovedProductsAsync(regionId: 1, certType: CertificationType.Ocop);
            Assert.NotEmpty(northOcopProducts);
            Assert.All(northOcopProducts, p =>
            {
                Assert.Contains(p.ProvinceId, northProvinceIds);
                Assert.Contains(p.Certifications, c => c.Type == CertificationType.Ocop && c.IsVerified);
            });
        }
    }
    [Fact]
    public async Task HarvestBatchRepository_ShouldLoadLogs_AndEnforceUniqueBatchCode()
    {
        var (conn, db) = await CreateSqliteDbAsync();
        await using (conn)
        await using (db)
        {
            await AgriTraceDbSeeder.SeedAsync(db);
            var batchRepo = new EfHarvestBatchRepository(db);
            var existing = await batchRepo.GetByBatchCodeAsync("lot-tn-2026-001");
            Assert.NotNull(existing);
            Assert.Equal(2, existing!.FarmingLogs.Count);
            Assert.True(await batchRepo.ExistsBatchCodeAsync("LOT-TN-2026-001"));

            // Thử thêm 1 lô trùng BatchCode -> phải văng DbUpdateException do Unique Index
            var duplicate = HarvestBatch.Create(existing.ProductId, "LOT-TN-2026-001", 100m);
            await batchRepo.AddAsync(duplicate);

            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
    }
    [Fact]
    public async Task OrderRepository_ShouldFindExpiredOrders_AndEnforceUniqueIdempotencyKey()
    {
        var (conn, db) = await CreateSqliteDbAsync();
        await using (conn)
        await using (db)
        {
            await AgriTraceDbSeeder.SeedAsync(db);
            var product = await db.AgriProducts.FirstAsync();
            var orderRepo = new EfOrderRepository(db);
            var now = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);

            // Tạo đơn từ 20 phút trước (đã quá hạn thanh toán 15 phút)
            var expiredOrder = Order.Create(Guid.NewGuid(), "TPHCM", "0901234567", now.AddMinutes(-20), "IDEMP_KEY_001");
            expiredOrder.AddItem(OrderItem.Create(expiredOrder.Id, product.Id, product.Name, product.Price, 1));
            await orderRepo.AddAsync(expiredOrder);
            await db.SaveChangesAsync();

            var expiredList = await orderRepo.GetExpiredPendingOrdersAsync(now);
            Assert.Single(expiredList);
            Assert.Single(expiredList[0].Items);

            // Thử tạo đơn thứ 2 trùng IdempotencyKey -> phải văng DbUpdateException
            var dupOrder = Order.Create(Guid.NewGuid(), "Hà Nội", "0909999999", now, "IDEMP_KEY_001");
            await orderRepo.AddAsync(dupOrder);

            await Assert.ThrowsAnyAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }
    }
    [Fact]
    public async Task UnitOfWork_RollbackTransaction_ShouldNotPersistChanges()
    {
        var (conn, db) = await CreateSqliteDbAsync();
        await using (conn)
        await using (db)
        {
            var uow = new EfUnitOfWork(db);
            var reportRepo = new EfViolationReportRepository(db);
            await uow.BeginTransactionAsync();
            var report = ViolationReport.Create(Guid.NewGuid(), ReportTargetType.Product, Guid.NewGuid(), "Test rollback");
            await reportRepo.AddAsync(report);
            await uow.SaveChangesAsync();
            await uow.RollbackTransactionAsync();
            db.ChangeTracker.Clear();
            Assert.Empty(await reportRepo.GetOpenReportsAsync());
        }
    }
}