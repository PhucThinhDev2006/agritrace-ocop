using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
namespace AgriTrace.Infrastructure.Identity;

public class JwtOptions
{
    public const string SectionName ="Jwt";
    public string Issuer {get; set;} = "AgriTrace.API";
    public string Audience {get; set;} = "AgriTrace.Client";
    public string SecretKey {get; set;} = string.Empty;
    public int AccessTokenMinutes {get; set;} = 60; // 60s
    public int RefreshTokenDays {get; set;} = 7; // 7 day
}
public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAtUtc) CreateAccessToken(AppUser user, IEnumerable<string> roles);
    string GenerateRefreshToken();
    string HashRefreshToken(string rawToken);
    int RefreshTokenDays {get;}
}
public class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;
    public int RefreshTokenDays => _options.RefreshTokenDays;
    public (string Token, DateTime ExpiresAtUtc) CreateAccessToken(AppUser user, IEnumerable<string> roles)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName)
        };
        foreach(var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: creds
        );
        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }
    public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    public string HashRefreshToken(string rawToken)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(bytes);
    }
}