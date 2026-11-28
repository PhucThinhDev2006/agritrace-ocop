using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
namespace AgriTrace.Infrastructure.Identity;
/// <summary> Băm mật khẩu chuẩn Argon2id + Salt ngẫu nhiên 16 byte (thay thế PBKDF2 mặc định của Identity) </summary>
public class Argon2idPasswordHasher : IPasswordHasher<AppUser>
{
    private const int SaltSize = 16; // 16 byte Salt
    private const int HashSize = 32; // 32 byte ouput hash
    private const int DegreeOfParallelism = 2;
    private const int Iterations = 3;
    private const int MemorySizeKb = 65536; // 64MB RAM
    public string HashPassword(AppUser user, string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = ComputeHash(password, salt, DegreeOfParallelism, Iterations, MemorySizeKb);
        return $"$argon2id$v=19$m={MemorySizeKb},t={Iterations},p={DegreeOfParallelism}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }
    public PasswordVerificationResult VerifyHashedPassword(AppUser user, string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrEmpty(providedPassword))
        {
            return PasswordVerificationResult.Failed;
        }

        // Định dạng: $argon2id$v=19$m=65536,t=3,p=2$<saltBase64>$<hashBase64>
        var parts = hashedPassword.Split('$', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 5 || parts[0] != "argon2id")
        {
            return PasswordVerificationResult.Failed;
        }

        try
        {
            var paramParts = parts[2].Split(',');
            var m = int.Parse(paramParts[0].Split('=')[1]);
            var t = int.Parse(paramParts[1].Split('=')[1]);
            var p = int.Parse(paramParts[2].Split('=')[1]);
            var salt = Convert.FromBase64String(parts[3]);
            var expectedHash = Convert.FromBase64String(parts[4]);
            var actualHash = ComputeHash(providedPassword, salt, p, t, m);
            return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash)? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
        }
        catch
        {
            return PasswordVerificationResult.Failed;
        }
    }
    private static byte[] ComputeHash(string password, byte[] salt, int parallelism, int iterations, int memorySizeKb)
    {
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
        {
            Salt = salt,
            DegreeOfParallelism = parallelism,
            Iterations = iterations,
            MemorySize = memorySizeKb
        };
        return argon2.GetBytes(HashSize);
    }
}