namespace AgriTrace.Domain.Exceptions;

/// <sumary> Lỗi vi phạm quy tắc nghiệp vụ (map 400 Bad Request) </sumary>
public sealed class BusinessRuleExceptions(string message) : Exception(message);

///<sumary> Không tìm thấy tài nguyên (map 404 Not Found) </sumary>
public sealed class NotFoundException(string message) : Exception(message);

/// <sumary> Xung đột dữ liệu: Hết hàng, trung mã, báo cáo đã đóng (map 409 Confict) </smary>
public sealed class ConfictException(string message) : Exception(message);