using AgriTrace.Domain.Exceptions;
namespace AgriTrace.Domain.Entities;

/// <summary> Chi tiết từng sản phẩm trong 1 đơn hàng (snapshot tên và đơn giá lúc mua) </summary>
public class OrderItem
{
    public Guid Id {get; private set;} = Guid.NewGuid();
    public Guid OrderId {get; private set;}
    public Guid ProductId {get; private set;}
    public string ProductName {get; private set;} = string.Empty;
    public decimal UnitPrice {get; private set;}
    public int Quantity {get; private set;}
    
    /// <summary> Thành tiền của dòng sản phẩm </summary>
    public decimal Subtotal => UnitPrice * Quantity;
    private OrderItem() {}
    public static OrderItem Create(Guid orderId, Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (orderId == Guid.Empty)
        {
            throw new ArgumentException("OrderId không hợp lệ!", nameof(orderId));
        }
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("ProductId không hợp lệ!", nameof(productId));
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(productName);
        if (unitPrice <= 0)
        {
            throw new BusinessRuleException("Đơn giá sản phẩm phải lớn hơn 0.");
        }
        if (quantity <= 0)
        {
            throw new BusinessRuleException("Số lượng đặt mua phải lớn hơn 0!");
        }
        return new OrderItem
        {
            OrderId = orderId,
            ProductId = productId,
            ProductName = productName.Trim(),
            UnitPrice = unitPrice,
            Quantity = quantity
        };
    }
}