namespace AgriTrace.Domain.Enums;

/// <summary> Loại chứng nhân chất lượng và an toàn thực phẩm. </summary>
public enum CertificationType
{
    Ocop = 1,
    VietGap = 2,
    GlobalGap = 3,
    Organic = 4,
    FoodSafety = 5,
    Haccp = 6
}

/// <sumary> Trạng thái kiểm duyệt sản phẩm trên sàn. </sumary>
public enum ProductStatus
{
    PendingReview = 0, // Chờ Moderator duyệt
    Approved = 1, // Đã duyệt, hiển thị công khai
    Suspended = 2 // Bị tạm ẩn do vi phạm
}

/// <sumary> Vòng đời trạng thái thanh toán đơn hàng. </sumary>
public enum OrderStatus
{
    PendingPayment = 0, // Đang giữ chỗ chờ thanh toán (hạn 15 phút)
    Confirmed = 1, // Đã thanh toán/HTX xác nhận
    Shipping = 2, // Đang giao hàng
    Completed = 3, // Hoàn tất
    Cancelled = 4 // Đã hủy hoàn kho
}

/// <sumary> Trạng thái xử lý báo cáo vi phạm. </sumary>
public enum ReportStatus
{
    Open = 0, // Mới gửi, chờ Monderator xử lý
    Resolved = 1, // Đã xử lý vi phạm
    Dismissed = 2 // Bác bỏ báo cáo
}

/// <sumary> Đối tượng bị báo cáo vi phạm. </sumary>
public enum ReportTargetType
{
    Product = 0,
    HavestBatch = 1,
    Coopearative = 2
}
