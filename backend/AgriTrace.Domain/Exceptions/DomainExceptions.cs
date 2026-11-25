namespace AgriTrace.Domain.Exceptions;

/// <summary> Lỗi vi phạm quy tắc nghiệp vụ (map 400 Bad Request) </summary>
public sealed class BusinessRuleException(string message) : Exception(message);

///<summary> Không tìm thấy tài nguyên (map 404 Not Found) </summary>
public sealed class NotFoundException(string message) : Exception(message);

/// <summary> Xung đột dữ liệu: Hết hàng, trung mã, báo cáo đã đóng (map 409 Confict) </summary>
public sealed class ConflictException(string message) : Exception(message);