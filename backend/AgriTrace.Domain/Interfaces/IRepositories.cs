using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Enums;
namespace AgriTrace.Domain.Interfaces;

public interface IRegionRepository
{
    Task<IReadOnlyList<Region>> GetAllRegionsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Province>> GetProvincesByRegionAsync(int regionId, CancellationToken ct = default);
    Task<Province?> GetProvinceByIdAsync(int provinceId, CancellationToken ct = default);
}

public interface ICooperativeRepository
{
    Task<Cooperative?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Cooperative?> GetByOwnerUserIdAsync(Guid ownerUserId, CancellationToken ct = default);
    Task AddAsync(Cooperative cooperative, CancellationToken ct = default);
}

public interface IProductRepository
{
    Task<AgriProduct?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AgriProduct?> GetByIdWithCertificationsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<AgriProduct>> GetApprovedProductsAsync(int? regionId = null, int? provinceId = null, CertificationType? certType = null, CancellationToken ct = default);
    Task AddAsync(AgriProduct product, CancellationToken ct = default);
}

public interface IHarvestBatchRepository
{
    Task<HarvestBatch?> GetByIdWithLogsAsync(Guid id, CancellationToken ct = default);
    Task<HarvestBatch?> GetByBatchCodeAsync(string batchCode, CancellationToken ct = default);
    Task<bool> ExistsBatchCodeAsync(string batchCode, CancellationToken ct = default);
    Task AddAsync(HarvestBatch batch, CancellationToken ct = default);
}

public interface IOrderRepository
{
    Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetByBuyerIdAsync(Guid buyerUserId, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> GetExpiredPendingOrdersAsync(DateTime utcNow, CancellationToken ct = default);
    Task AddAsync(Order order, CancellationToken ct = default);
}

public interface IViolationReportRepository
{
    Task<ViolationReport> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ViolationReport>> GetOpenReportsAsync(CancellationToken ct = default);
    Task AddAsync(ViolationReport report, CancellationToken ct = default);
}