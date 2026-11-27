namespace AgriTrace.Domain.Entities;
/// <summary> 3 vùng miền </summary>
public class Region
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    private Region() { }
    public static Region Create(int id, string name, string code)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Id vùng phải lớn hơn 0!");
        }
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        return new Region
        {
            Id = id,
            Name = name.Trim(),
            Code = code.Trim().ToUpperInvariant()
        };
    }
}