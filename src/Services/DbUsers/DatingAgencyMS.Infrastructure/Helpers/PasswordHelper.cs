using System.Security.Cryptography;
using System.Text;

namespace DatingAgencyMS.Infrastructure.Helpers;

public static class PasswordHelper
{
    private const int KeySize = 64;
    private const int Iterations = 350000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    public static (string hashedPassword, string salt) HashPasword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(KeySize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithm,
            KeySize);

        return (Convert.ToHexString(hash), Convert.ToHexString(salt));
    }
    
    public static bool VerifyPassword(string password, string hash, string salt)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, Encoding.UTF8.GetBytes(salt), Iterations, HashAlgorithm, KeySize);

        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }
}