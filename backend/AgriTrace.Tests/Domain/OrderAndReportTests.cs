using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Enums;
using AgriTrace.Domain.Exceptions;
namespace AgriTrace.Tests.Domain;

public class OrderAndReportTests
{
    [Fact]
    public void Confirm_ShouldThrowConflict_WhenPast15MinuteDeadline()
    {
        var now = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var order = Order.Create(Guid.NewGuid(), "123 lê Lợi, Q1, TPHCM", "0901234567", now);
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "Chè Tân Cương", 200_000m, 2));
        Assert.Throws<ConflictException>(() => order.Confirm(now.AddMinutes(16)));
    }
    [Fact]
    public void OrderLifecycle_PendingToCompleted_ShouldSucceed()
    {
        var now = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var order = Order.Create(Guid.NewGuid(), "45 Nguyễn Huệ, Huế", "0912345678", now);
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "Mè xửng Huế", 50_000m, 3));
        order.Confirm(now.AddMinutes(5));
        order.MarkAsShipping();
        order.Complete();
        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.Equal(150_000m, order.TotalAmount);
    }
    [Fact]
    public void Cancel_ShouldThrow_WhenOrderIsShipping()
    {
        var now = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var order = Order.Create(Guid.NewGuid(), "Hà Nội", "0987654321", now);
        order.AddItem(OrderItem.Create(order.Id, Guid.NewGuid(), "Cốm Làng Vòng", 120_000m, 1));
        order.Confirm(now.AddMinutes(2));
        order.MarkAsShipping();
        Assert.Throws<BusinessRuleException>(() => order.Cancel("Đổi ý không mua nữa"));
    }
    [Fact]
    public void ViolationReport_ResolveTwice_ShouldThrowConflict()
    {
        var now = DateTime.UtcNow;
        var report = ViolationReport.Create(Guid.NewGuid(), ReportTargetType.Product, Guid.NewGuid(), "Nghi ngờ giả mạo tem OCOP");
        report.Resolve(Guid.NewGuid(), "Đã xác minh và tạm ẩn sản phẩm", now);
        Assert.Equal(ReportStatus.Resolved, report.Status);
        Assert.Throws<ConflictException>(() => report.Resolve(Guid.NewGuid(), "Xử lý tại lần 2", now.AddMinutes(1)));
    }
}