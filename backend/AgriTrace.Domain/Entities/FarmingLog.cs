using AgriTrace.Domain.Exceptions;
namespace AgriTrace.Domain.Entities;

/// <summary> Nhật ký canh tác của 1 lô thu hoạch </summary>
public class FarmingLog
{
    public Guid Id {get; private set;} = Guid.NewGuid();
    public Guid HarvestBatchId {get; private set;}
    public DateTime ActivityDate {get; private set;}
    public string ActivityType {get; private set;} = string.Empty; // Gieo trồng, bón phân, phun thuốc
    public string Description {get; private set;} = string.Empty;
    public string? InputMaterial {get; private set;} // Tên phân bón, thuốc BVTV
    public int PreHarvestIntervalDays {get; private set;} // Thời gian cách ly an toàn
    public string? PhotoUrl {get; private set;} // Ảnh minh chứng 
    public DateTime CreatedAt {get; private set;} = DateTime.UtcNow;

    /// <summary> Ngày thu hoạch an toàn sớm nhất tính theo thời gian cách ly </summary>
    public DateTime EarliestSafeDate => ActivityDate.AddDays(PreHarvestIntervalDays);
    private FarmingLog() {}
    public static FarmingLog Create(Guid harvestBatchId, DateTime activityDate, string activityType, string description, int preHarvestIntervalDays = 0, string? inputMaterial = null, string? photoUrl = null)
    {
        if (harvestBatchId == Guid.Empty)
        {
            throw new ArgumentException("HarvestBatchId không hợp lệ!", nameof(harvestBatchId));
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(activityType);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        if (preHarvestIntervalDays < 0)
        {
            throw new BusinessRuleException("Thời gian cách ly không được âm.");
        }
        return new FarmingLog
        {
            HarvestBatchId = harvestBatchId,
            ActivityDate = activityDate,
            ActivityType = activityType.Trim(),
            Description = description.Trim(),
            PreHarvestIntervalDays = preHarvestIntervalDays,
            InputMaterial = string.IsNullOrWhiteSpace(inputMaterial)? null : inputMaterial.Trim(),
            PhotoUrl = string.IsNullOrWhiteSpace(photoUrl)? null : photoUrl.Trim()
        };
    }
}