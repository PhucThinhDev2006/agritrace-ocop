using AgriTrace.Domain.Enums;
using AgriTrace.Domain.Exceptions;
namespace AgriTrace.Domain.Entities;

/// <summary> Báo cáo vi phạm sản phẩm, lô thu hoạch hoặc HTX </summary>
public class ViolationReport
{
    public Guid Id {get; private set;} = Guid.NewGuid();
    public Guid ReporterUserId {get; private set;}
    public ReportTargetType TargetType {get; private set;}
    public Guid TargetId {get; private set;}
    public string Reason {get; private set;} = string.Empty;
    public string? EvidenceUrl {get; private set;}
    public ReportStatus Status {get; private set;} = ReportStatus.Open;
    public Guid? ResolvedByModeratorId {get; private set;}
    public string? ResolutionNote {get; private set;}
    public DateTime CreatedAt {get; private set;} = DateTime.UtcNow;
    public DateTime? ResolvedAt {get; private set;}
    private ViolationReport() {}
    public static ViolationReport Create(Guid reporterUserId, ReportTargetType targetType, Guid targetId, string reason, string? evidenceUrl = null)
    {
        if (reporterUserId == Guid.Empty)
        {
            throw new ArgumentException("ReporterUserId không hợp lệ!", nameof(reporterUserId));
        }
        if (targetId == Guid.Empty)
        {
            throw new ArgumentException("TargetId không hợp lệ!", nameof(targetId));
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        return new ViolationReport
        {
            ReporterUserId = reporterUserId,
            TargetType = targetType,
            TargetId = targetId,
            Reason = reason.Trim(),
            EvidenceUrl = string.IsNullOrWhiteSpace(evidenceUrl)? null : evidenceUrl.Trim(),
            Status = ReportStatus.Open
        };
    }

    /// <summary> Moderator xác nhận vi phạm và ghi kết quả xử lý </summary>
    public void Resolve(Guid moderatorId, string resolutionNote, DateTime utcNow)
    {
        if (Status != ReportStatus.Open)
        {
            throw new ConflictException("Báo cáo vi phạm này đã được xử lý trước đó.");
        }
        if (moderatorId == Guid.Empty)
        {
            throw new ArgumentException("ModeratorId không hợp lệ!", nameof(moderatorId));
        }
        if (string.IsNullOrWhiteSpace(resolutionNote))
        {
            throw new BusinessRuleException("Phải nhập ghi chú kết quả xử lý vi phạm.");
        }
        Status = ReportStatus.Resolved;
        ResolvedByModeratorId = moderatorId;
        ResolutionNote = resolutionNote.Trim();
        ResolvedAt = utcNow;
    }

    /// <summary> Moderator bác bỏ báo cáo không đủ căn cứ </summary>
    public void Dismiss(Guid moderatorId, string resolutionNote, DateTime utcNow)
    {
        if (Status != ReportStatus.Open)
        {
            throw new ConflictException("Báo cáo vi phạm này đã được xử lý trước đó.");
        }
        if (moderatorId == Guid.Empty)
        {
            throw new ArgumentException("ModeratorId không hợp lệ!", nameof(moderatorId));
        }
        if (string.IsNullOrWhiteSpace(resolutionNote))
        {
            throw new BusinessRuleException("Phải nhập lý do bác bỏ báo cáo.");
        }
        Status = ReportStatus.Dismissed;
        ResolvedByModeratorId = moderatorId;
        ResolutionNote = resolutionNote.Trim();
        ResolvedAt = utcNow;
    }
}