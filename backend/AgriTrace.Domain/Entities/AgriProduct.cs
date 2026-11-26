using AgriTrace.Domain.Enums;
using AgriTrace.Domain.Exceptions;
namespace AgriTrace.Domain.Entities;

/// <summary> Sản phẩm nông sản </summary>
public class AgriProduct
{
    private readonly List<ProductCertification> _certifications = new();
    public Guid Id {get; private set;} = Guid.NewGuid();
    public Guid CooperativeId {get; private set;}
    public int ProvinceId {get; private set;}
    public string Name {get; private set;} = string.Empty;
    public string Description {get; private set;} = string.Empty;
    public decimal Price {get; private set;}
    public int StockQuantity {get; private set;}
    public string NutritionInfoJson {get; private set;} = "{}"; // Lưu theo cặp key-value
    public ProductStatus Status {get; private set;} = ProductStatus.PendingReview;
    public string? SuspensionReason {get; private set;}
    public DateTime CreatedAt {get; private set;} = DateTime.UtcNow;

    /// <summary> Chỉ cho đọc danh sách chứng nhận từ bên ngoài, không cho tư ý Add/Remove </summary>
    public IReadOnlyCollection<ProductCertification> Certifications => _certifications.AsReadOnly();
    private AgriProduct() {}
    public static AgriProduct Create(Guid cooperativeId, int provinceId, string name, string description, decimal price, int stockQuantity, string? nutritionInfoJson = null)
    {
        if (cooperativeId == Guid.Empty)
        {
            throw new ArgumentException("CooperativeId không hợp lệ!", nameof(cooperativeId));
        }
        if (provinceId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(provinceId), "ProvinceId không hợp lệ!");
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        if (price <= 0)
        {
            throw new BusinessRuleException("Giá trị sản phẩm phải lớn hơn 0!");
        }
        if (stockQuantity < 0)
        {
            throw new BusinessRuleException("Số lượng tồn kho không được âm!");
        }
        return new AgriProduct
        {
            CooperativeId = cooperativeId,
            ProvinceId = provinceId,
            Name = name.Trim(),
            Description = description.Trim(),
            Price = price,
            StockQuantity = stockQuantity,
            NutritionInfoJson = string.IsNullOrWhiteSpace(nutritionInfoJson) ? "{}" : nutritionInfoJson.Trim(),
            Status = ProductStatus.PendingReview
        };
    }
    public void AddCertification(ProductCertification certification)
    {
        ArgumentNullException.ThrowIfNull(certification);
        if (certification.ProductId != Id)
        {
            throw new BusinessRuleException("Chứng nhận không thuộc về sản phẩm này!");
        }
        _certifications.Add(certification);
    }

    /// <summary> Moderator duyệt sản phẩm lên sàn </summary>
    public void Approve()
    {
        Status = ProductStatus.Approved;
        SuspensionReason = null;
    }

    /// <summary> Moderator tạm ẩn sản phẩm vi phạm </summary>
    public void Suspend(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BusinessRuleException("Phải nhập lý do tạm ẩn sản phẩm");
        }
        Status = ProductStatus.Suspended;
        SuspensionReason = reason.Trim();
    }

    /// <summary> Trừ kho khi đặt hàng kiểm tra tồn kho không âm </summary>
    public void DeductStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new BusinessRuleException("Số lượng mua phải lớn hơn 0!");
        }
        if (StockQuantity < quantity)
        {
            throw new ConflictException("Sản phẩm không đủ tồn kho.");
        }
        StockQuantity -= quantity;
    }

    /// <summary> Hoàn lại kho khi hủy đơn hoặc quá hạn thanh toán 15 phút </summay>
    public void RestoreStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new BusinessRuleException("Sản phẩm hoàn kho phải lớn hơn 0.");
        }
        StockQuantity += quantity;
    }
}