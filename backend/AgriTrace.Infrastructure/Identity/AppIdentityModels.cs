using Microsoft.AspNetCore.Identity;
namespace AgriTrace.Infrastructure.Identity;
/// <summary> 4 vai trò chuẩn của hệ thống AgriTrace OCOP </summary>
public static class AppRoles
{
    public const string Buyer = "Buyer";
    public const string Cooperative = "Cooperative";
    public const string Moderator = "Moderator";
    public const string Admin = "Admin";
    public static readonly string[] All = [Buyer, Cooperative, Moderator, Admin];
}
/// <summary> Tài khoản người dùng mở rộng từ ASP.NET Core Identity - khóa chính là Guid </summary>
public class AppUser : IdentityUser<Guid>
{
    public string FullName {get; set;} = string.Empty;
    public bool IsActive {get; set;} = true;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
}
/// <summary> Vai trò người dùng - khóa chính Guid </summary>
public class AppRole : IdentityRole<Guid>
{
    public AppRole() {}
    public AppRole(string roleName) : base(roleName) {}
}
/// <summary> Lưu SHA-256 hash của Refresh Tokenn (7 ngày) phục vụ Rotation và Logout </summary>
public class RefreshToken
{
    public Guid Id {get; set;} = Guid.NewGuid();
    public Guid UserId {get; set;}
    public string TokenHash {get; set;} = string.Empty;
    public DateTime ExpiresAtUtc {get; set;}
    public DateTime CreatedAtUtc {get; set;} = DateTime.UtcNow;
    public DateTime? RevokedAtUtc {get; set;}
    public string? ReplacedByTokenHash {get; set;}
    public bool IsRevoked => RevokedAtUtc.HasValue;
    public bool IsActiveAt(DateTime utcNow) => !IsRevoked && ExpiresAtUtc > utcNow;
}