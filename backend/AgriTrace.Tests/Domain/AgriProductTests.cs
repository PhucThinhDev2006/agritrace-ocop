using AgriTrace.Domain.Entities;
using AgriTrace.Domain.Enums;
using AgriTrace.Domain.Exceptions;
namespace AgriTrace.Tests.Domain;

public class AgriProductTests
{
    [Fact]
    public void DeductStock_ShouldThrowConflict_WhenInsufficientStock()
    {
        var product = AgriProduct.Create(Guid.NewGuid(), 1, "Chè Tân Cương", "Đặc sản Thái Nguyên", 250_000m, 5);
        Assert.Throws<ConflictException>(() => product.DeductStock(10));
    }
    [Fact]
    public void DeductAndRestoreStock_ShouldUpdateStockCorrectly()
    {
        var product = AgriProduct.Create(Guid.NewGuid(), 1, "Mật ong Bạc Hà", "Đặc sản Hà Giang", 350_000m, 20);
        product.DeductStock(7);
        product.RestoreStock(2);
        Assert.Equal(15, product.StockQuantity);
    }
    [Fact]
    public void Suspend_ShouldThrow_WhenReasonEmpty()
    {
        var product = AgriProduct.Create(Guid.NewGuid(), 1, "Gạo ST25", "Đặc sản Sóc Trăng", 180_000m, 150);
        Assert.Throws<BusinessRuleException>(() => product.Suspend(" "));
    }
    [Fact]
    public void ProductCertification_IsValidAt_ShouldRequireVerifiedAndNotExpired()
    {
        var issued = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var expiry = new DateTime(2027, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var cert = ProductCertification.Create(Guid.NewGuid(), CertificationType.Ocop, "4 sao", "UBND Tỉnh", issued, expiry);
        Assert.False(cert.IsValidAt(new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc)));
        cert.Verify();
        Assert.True(cert.IsValidAt(new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc)));
        Assert.False(cert.IsValidAt(new DateTime(2027, 2, 1, 0, 0, 0, DateTimeKind.Utc)));
    }
}