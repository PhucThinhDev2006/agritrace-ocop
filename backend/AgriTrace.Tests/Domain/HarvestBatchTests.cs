using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Exceptions;
namespace AgriTrace.Tests.Domain;

public class HarvestBatchTests
{
    [Fact]
    public void LockAndGenerateQr_ShouldThrow_WhenHarvestBeforePhiSafeDate()
    {
        var batch = HarvestBatch.Create(Guid.NewGuid(), "lot-2026-001", 500m);
        var sprayDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        var log = FarmingLog.Create(batch.Id, sprayDate, "Phun thuốc BVTV", "Phun phòng sâu", preHarvestIntervalDays: 14, inputMaterial: "Thuốc sinh học A");
        batch.AddFarmingLog(log);
        var tooEarlyHarvestDate = new DateTime(2026, 5, 10, 0, 0, 0, DateTimeKind.Utc);
        Assert.Throws<BusinessRuleException>(() => batch.LockAndGenerateQr(tooEarlyHarvestDate, "HMAC_SIG_123"));
        Assert.False(batch.IsLocked);
    }
    [Fact]
    public void LockAndGenerateQr_ShouldSucceed_WhenPhiSatisfied()
    {
        var batch = HarvestBatch.Create(Guid.NewGuid(), "lot-2026-002", 300m);
        var sprayDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        var log = FarmingLog.Create(batch.Id, sprayDate, "Phun thuốc BVTV", "Phun phòng sâu", preHarvestIntervalDays: 14);
        batch.AddFarmingLog(log);
        var safeHarvestDate = new DateTime(2026, 5, 16, 0, 0, 0, DateTimeKind.Utc);
        batch.LockAndGenerateQr(safeHarvestDate, "HMAC_SIG_VALID");
        Assert.True(batch.IsLocked);
        Assert.Equal("LOT-2026-002", batch.BatchCode);
        Assert.Equal("HMAC_SIG_VALID", batch.QrSignature);
    }
    [Fact]
    public void AddFarmingLog_ShouldThrow_WhenBatchIsAlreadyLocked()
    {
        var batch = HarvestBatch.Create(Guid.NewGuid(), "LOT-2026-003", 200m);
        var date = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        batch.AddFarmingLog(FarmingLog.Create(batch.Id, date, "Gieo trồng", "Xuống giống"));
        batch.LockAndGenerateQr(date.AddDays(30), "HMAC_SIG_LOCKED");
        var extraLog = FarmingLog.Create(batch.Id, date.AddDays(31), "Bón phân", "Ghi chèn sau khi khóa");
        Assert.Throws<BusinessRuleException>(() => batch.AddFarmingLog(extraLog));
    }
    [Fact]
    public void LockAndGenerateQr_ShouldThrow_WhenNoFarmingLogs()
    {
        var batch = HarvestBatch.Create(Guid.NewGuid(), "LOT-2026-004", 100m);
        Assert.Throws<BusinessRuleException>(() => batch.LockAndGenerateQr(DateTime.UtcNow, "HMAC_SIG"));
    }
}