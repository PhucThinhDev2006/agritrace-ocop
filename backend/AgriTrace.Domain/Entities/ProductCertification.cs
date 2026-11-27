using AgriTrace.Domain.Enums;
using AgriTrace.Domain.Exceptions;
namespace AgriTrace.Domain.Entities;

/// <summary> Chứng nhận chất lượng và an toàn thực phẩm của sản phẩm </summary>
public class ProductCertification
{
    public Guid Id {get; private set;} = Guid.NewGuid();
    public Guid ProductId {get; private set;}
    public CertificationType Type {get; private set;}
    public string GradeOrCode {get; private set;} = string.Empty;
    public string IssuedBy {get; private set;} = string.Empty;
    public DateTime IssuedDate {get; private set;}
    public DateTime ExpiryDate {get; private set;}
    public bool IsVerified {get; private set;}
    private ProductCertification() {}
    public static ProductCertification Create (Guid productId, CertificationType type, string gradeOrCode, string issuedBy, DateTime issuedDate, DateTime expiryDate)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("ProductId không hợp lệ!", nameof(productId));
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(gradeOrCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(issuedBy);
        if (expiryDate <= issuedDate)
        {
            throw new BusinessRuleException("Ngày hết hạn chứng nhận phải sau ngày cấp.");
        }
        return new ProductCertification
        {
            ProductId = productId,
            Type = type,
            GradeOrCode = gradeOrCode.Trim(),
            IssuedBy = issuedBy.Trim(),
            IssuedDate = issuedDate,
            ExpiryDate = expiryDate,
            IsVerified = false // Chờ Admin xác nhận
        };
    }

    /// <summary> Admin xác nhận chứng nhận hợp lệ </summary>
    public void Verify() => IsVerified = true;

    /// <summary> Kiểm tra chứng nhận còn hiệu lực ở thời điểm hiện tại hay không </summary>
    public bool IsValidAt(DateTime utcNow) => IsVerified && ExpiryDate > utcNow;
}