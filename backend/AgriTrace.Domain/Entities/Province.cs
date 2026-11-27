namespace AgriTrace.Domain.Entities;
/// <summary> 63 tỉnh </summary>
public class Province
{
    public int Id { get; private set; }
    public int RegionId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    private Province() { }
    public static  Province Create(int id, int regionId, string name, string code)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id tỉnh thành phải lớn hơn 0!");
        }
        if (regionId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(regionId), "RegionId không hợp lệ!");
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        return new Province
        {
            Id = id,
            RegionId = regionId,
            Name = name.Trim(),
            Code = code.Trim().ToUpperInvariant()
        };
    }
}