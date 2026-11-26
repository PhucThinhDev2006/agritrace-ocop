using AgriTrace.Domain.Exceptions;
namespace AgriTrace.Domain.Entities;

/// <summary> Lô thu hoạch sản phẩm - kiểm tra thời gian cách ly và khóa bất biến khi phát hành QR </summary>
public class HarvestBatch
{
    private readonly List<FarmingLog> _farmingLogs = new();
    public Guid Id {get; private set;} = Guid.NewGuid();
    public Guid ProductId {get; private set;}
    public string BatchCode {get; private set;} = string.Empty; // LOT-2026-001
    public DateTime? HarvestDate {get; private set;}
    public decimal ExpectedYieldKg {get; private set;}
    public bool IsLocked {get; private set;}
    public string? QrSignature {get; private set;} // Chữ ký HMAC-SHA256 chống giả mạo QR
    public DateTime CreatedAt {get; private set;} = DateTime.UtcNow;

    /// <summary> Chỉ cho đọc danh sách nhật ký từ bên ngoài, không cho tự ý Add/Remove </summary>
    public IReadOnlyCollection<FarmingLog> FarmingLogs => _farmingLogs.AsReadOnly();
    private HarvestBatch() {}
    public static HarvestBatch Create(Guid productId, string batchCode, decimal expectedYieldKg)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("ProductId không hợp lệ!", nameof(productId));
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(batchCode);
        if (expectedYieldKg <= 0)
        {
            throw new BusinessRuleException("Sản lượng dự kiến phải lớn hơn 0.");
        }
        return new HarvestBatch
        {
            ProductId = productId,
            BatchCode = batchCode.Trim().ToUpperInvariant(),
            ExpectedYieldKg = expectedYieldKg,
            IsLocked = false
        };
    }

    /// <summary> Ghi thêm 1 dòng nhật ký canh tác - bị chặn lại nếu lô đã khóa </summary>
    public void AddFarmingLog(FarmingLog log)
    {
        if (IsLocked)
        {
            throw new BusinessRuleException("Lô thu hoạch đã khóa bất biến, không thể thêm nhật ký canh tác.");
        }
        ArgumentNullException.ThrowIfNull(log);
        if (log.HarvestBatchId != Id)
        {
            throw new BusinessRuleException("Nhật ký canh tác không thuộc về lô thu hoạch này.");
        }
        _farmingLogs.Add(log);
    }

    /// <summary> Khóa bất biến lô thu hoạch và gắn chữ ký Qr sau khi kiểm tra đủ thời gian cách ly </summary>
    public void LockAndGenerateQr(DateTime harvestDate, string qrSignature)
    {
        if (IsLocked)
        {
            throw new BusinessRuleException("Lô thu hoạch này đã được khóa trước đó.");
        }
        if (_farmingLogs.Count == 0)
        {
            throw new BusinessRuleException("Phải có ít nhất 1 nhật ký canh tác trước khi khóa lô thu hoạch.");
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(qrSignature);
        foreach (var log in _farmingLogs)
        {
            if (harvestDate < log.ActivityDate)
            {
                throw new BusinessRuleException("Ngày thu hoạch không được trước ngày ghi nhật ký canh tác.");
            }
            if (harvestDate <log.EarliestSafeDate)
            {
                throw new BusinessRuleException($"Chưa đủ thời gian cách ly an toàn (PHI = {log.PreHarvestIntervalDays} ngày) cho hoạt động '{log.ActivityType}'. Ngày thu hoạch sớm nhất hợp lệ là {log.EarliestSafeDate:yyyy-MM-dd}.");
            }
        }
        HarvestDate = harvestDate;
        QrSignature = qrSignature.Trim();
        IsLocked = true;
    }
}