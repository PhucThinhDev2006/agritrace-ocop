using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Enums;
using AgriTrace.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace AgriTrace.Infrastructure.Persistence.Repositories;

public class EfRegionRepository(AgriTraceDbContext db) : IRegionRepository
{
    public async Task<IReadOnlyList<Region>> GetAllRegionsAsync(CancellationToken ct = default) => await db.Regions.AsNoTracking().OrderBy(r => r.Id).ToListAsync(ct);
    public async Task<IReadOnlyList<Province>> GetProvincesByRegionAsync(int regionId, CancellationToken ct = default) => await db.Provinces.AsNoTracking().Where(p => p.RegionId == regionId).OrderBy(p => p.Id).ToListAsync(ct);
    public Task<Province?> GetProvinceByIdAsync(int provienceId, CancellationToken ct = default) => db.Provinces.AsNoTracking().FirstOrDefaultAsync(p => p.Id == provienceId, ct);
}
public class EfCooperativeRepository(AgriTraceDbContext db) : ICooperativeRepository
{
    public Task<Cooperative?> GetByIdAsync(Guid Id, CancellationToken ct = default) => db.Cooperatives.FirstOrDefaultAsync(c => c.Id == Id, ct);
    public Task<Cooperative?> GetByOwnerUserIdAsync(Guid ownerUserId, CancellationToken ct = default) => db.Cooperatives.FirstOrDefaultAsync(c => c.OwnerUserId == ownerUserId, ct);
    public async Task AddAsync(Cooperative cooperative, CancellationToken ct = default) => await db.Cooperatives.AddAsync(cooperative, ct);
}
public class EfProductRepository(AgriTraceDbContext db) : IProductRepository
{
    public Task<AgriProduct?> GetByIdAsync(Guid Id, CancellationToken ct = default) => db.AgriProducts.FirstOrDefaultAsync(p => p.Id == Id, ct);
    public Task<AgriProduct?> GetByIdWithCertificationsAsync(Guid id, CancellationToken ct = default) => db.AgriProducts.Include(p => p.Certifications).FirstOrDefaultAsync(p => p.Id == id, ct);
    public async Task<IReadOnlyList<AgriProduct>> GetApprovedProductsAsync(int? regionId = null, int? provinceId = null, CertificationType? certType = null, CancellationToken ct = default)
    {
        IQueryable<AgriProduct> query = db.AgriProducts.AsNoTracking().Include(p => p.Certifications).Where(p => p.Status == ProductStatus.Approved);
        if (regionId.HasValue)
        {
            query = query.Where(p => db.Provinces.Any(pr => pr.Id == p.ProvinceId && pr.RegionId == regionId.Value));
        }
        if (provinceId.HasValue)
        {
            query = query.Where(p => p.ProvinceId == provinceId.Value);
        }
        if (certType.HasValue)
        {
            query = query.Where(p => p.Certifications.Any(c => c.Type == certType.Value && c.IsVerified));
        }
        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync(ct);
    }
    public async Task AddAsync(AgriProduct product, CancellationToken ct = default) => await db.AgriProducts.AddAsync(product, ct);
}
public class EfHarvestBatchRepository(AgriTraceDbContext db) : IHarvestBatchRepository
{
    public Task<HarvestBatch?> GetByIdWithLogsAsync(Guid id, CancellationToken ct = default) => db.HarvestBatches.Include(b => b.FarmingLogs).FirstOrDefaultAsync(b => b.Id == id, ct);
    public Task<HarvestBatch?> GetByBatchCodeAsync(string batchCode, CancellationToken ct = default)
    {
        var normalized = batchCode.Trim().ToUpperInvariant();
        return db.HarvestBatches.Include(b => b.FarmingLogs).FirstOrDefaultAsync(b => b.BatchCode == normalized, ct);
    }
    public Task<bool> ExistsBatchCodeAsync(string batchCode, CancellationToken ct = default)
    {
        var normalized = batchCode.Trim().ToUpperInvariant();
        return db.HarvestBatches.AnyAsync(b => b.BatchCode == normalized, ct);
    }
    public async Task AddAsync(HarvestBatch batch, CancellationToken ct = default) => await db.HarvestBatches.AddAsync(batch, ct);
}
public class EfOrderRepository(AgriTraceDbContext db) : IOrderRepository
{
    public Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default) => db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, ct);
    public async Task<IReadOnlyList<Order>> GetByBuyerIdAsync(Guid buyerUserId, CancellationToken ct = default) => await db.Orders.AsNoTracking().Include(o => o.Items).Where(o => o.BuyerUserId == buyerUserId).OrderByDescending(o => o.CreatedAt).ToListAsync(ct);
    public async Task<IReadOnlyList<Order>> GetExpiredPendingOrdersAsync(DateTime uctNow, CancellationToken ct = default) => await db.Orders.Include(o => o.Items).Where(o => o.Status == OrderStatus.PendingPayment && o.PaymentDeadlineUtc < uctNow).ToListAsync(ct);
    public async Task AddAsync(Order order, CancellationToken ct = default) => await db.Orders.AddAsync(order, ct);
}
public class EfViolationReportRepository(AgriTraceDbContext db) : IViolationReportRepository
{
    public Task<ViolationReport?> GetByIdAsync(Guid id, CancellationToken ct = default) => db.ViolationReports.FirstOrDefaultAsync(r => r.Id == id, ct);
    public async Task<IReadOnlyList<ViolationReport>> GetOpenReportsAsync(CancellationToken ct = default) => await db.ViolationReports.AsNoTracking().Where(r => r.Status == ReportStatus.Open).OrderBy(r => r.CreatedAt).ToListAsync(ct);
    public async Task AddAsync(ViolationReport report, CancellationToken ct = default) => await db.ViolationReports.AddAsync(report, ct);
}