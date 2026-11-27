namespace AgriTrace.Domain.Entities;
/// <summary> Hợp tác xã/Nhà cung cấp nông sản vùng miền </summary>
public class Cooperative
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid OwnerUserId { get; private set; }
    public int ProvinceId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public bool IsVerified { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    private Cooperative() { }
    public static Cooperative Create(Guid ownerUserId, int provinceId, string name, string address)
    {
        if (ownerUserId == Guid.Empty)
        {
            throw new ArgumentException("OwnerUserId không hợp lệ!", nameof(ownerUserId));
        }
        if (provinceId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(provinceId), "ProvinceId không hợp lệ!");
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(address);
        return new Cooperative
        {
            OwnerUserId = ownerUserId,
            ProvinceId = provinceId,
            Name = name.Trim(),
            Address = address.Trim(),
            IsVerified = false
        };
    }
    public void Verify() => IsVerified = true;
    public void UpdateInfo(string name, string address)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(address);
        Name = name.Trim();
        Address = address.Trim();
    }
}