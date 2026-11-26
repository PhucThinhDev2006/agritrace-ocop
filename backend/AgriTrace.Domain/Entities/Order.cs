using AgriTrace.Domain.Enums;
using AgriTrace.Domain.Exceptions;
namespace AgriTrace.Domain.Entities;

/// <summary> Đơn hàng của người mua - quản lý vòng đời trạng thái và hạn thanh toán 15 phút </summary>
public class Order
{
    private readonly List<OrderItem> _items = new();
    public Guid Id {get; private set;} = Guid.NewGuid();
    public Guid BuyerUserId {get; private set;}
    public string ShippingAddress {get; private set;} = string.Empty;
    public string ReceiverPhone {get; private set;} = string.Empty;
    public OrderStatus Status {get; private set;} = OrderStatus.PendingPayment;
    public decimal TotalAmount {get; private set;}
    public DateTime PaymentDeadlineUtc {get; private set;}
    public string? CancelReason {get; private set;}
    public DateTime CreatedAt {get; private set;}

    /// <summary> Chỉ cho đọc danh sách mặt hàng từ bên ngoài </summary>
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    private Order() {}
    public static Order Create(Guid buyerUserId, string shippingAddress, string receiverPhone, DateTime utcNow)
    {
        if (buyerUserId == Guid.Empty)
        {
            throw new ArgumentException("BuyerUserId không hợp lệ!", nameof(buyerUserId));
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(shippingAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(receiverPhone);
        return new Order
        {
            BuyerUserId = buyerUserId,
            ShippingAddress = shippingAddress.Trim(),
            ReceiverPhone = receiverPhone.Trim(),
            Status = OrderStatus.PendingPayment,
            TotalAmount = 0m,
            CreatedAt = utcNow,
            PaymentDeadlineUtc = utcNow.AddMinutes(15)
        };
    }
    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.PendingPayment)
        {
            throw new BusinessRuleException("Chỉ có thể thêm sản phẩm khi đơn hàng đang chờ thanh toán.");
        }
        ArgumentNullException.ThrowIfNull(item);
        if (item.OrderId != Id)
        {
            throw new BusinessRuleException("Chi tiết đơn hàng không thuộc về đơn hàng này.");
        }
        _items.Add(item);
        TotalAmount += item.Subtotal;
    } 

    /// <summary> Xác nhận đơn sau khi thanh toán thành công - hạn 15 phút </summary>
    public void Confirm(DateTime utcNow)
    {
        if (Status != OrderStatus.PendingPayment)
        {
            throw new ConflictException("Chỉ có thể xác nhận đơn hàng ở trạng thái chờ thanh toán.");
        }
        if (_items.Count == 0)
        {
            throw new BusinessRuleException("Đơn hàng phải có ít nhất 1 sản phẩm.");
        }
        if (utcNow > PaymentDeadlineUtc)
        {
            throw new ConflictException("Đơn hàng đã quá hạn thanh toán 15 phút.");
        }
        Status = OrderStatus.Confirmed;
    }

    /// <summary> Chuyển sang trạng thái đang giao hàng </summary>
    public void MarkAsShipping()
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new ConflictException("Chỉ có thể giao hàng khi đơn đã được xác nhận.");
        }
        Status = OrderStatus.Shipping;
    }

    /// <summary> Hoàn tất đơn hàng khi khách đã nhận hàng </summary>
    public void Complete()
    {
        if (Status != OrderStatus.Shipping)
        {
            throw new ConflictException("Chỉ có thể hoàn tất đơn hàng đang trong quá trình giao.");
        }
        Status = OrderStatus.Completed;
    }
    
    

    /// <summary> Hủy đơn hàng trước khi giao kèm lý do bắt buộc </summary>
    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Cancelled)
        {
            throw new ConflictException("Không thể hủy đơn hàng đã hoàn tất hoặc đã hủy trước đó.");
        }
        if (Status == OrderStatus.Shipping)
        {
            throw new BusinessRuleException("Không thể hủy đơn hàng trong quá trình giao.");
        }
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new BusinessRuleException("Phải nhập lý do hủy đơn hàng.");
        }
        Status = OrderStatus.Cancelled;
        CancelReason = reason.Trim();
    }
}