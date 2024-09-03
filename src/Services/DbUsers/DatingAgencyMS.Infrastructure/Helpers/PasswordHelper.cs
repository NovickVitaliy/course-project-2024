using System.Security.Cryptography;
using System.Text;

namespace DatingAgencyMS.Infrastructure.Helpers;

public static class PasswordHelper
{
    private const int KeySize = 64;
    private const int Iterations = 350000;
    private static readonly HashAlgorithmName _hashAlgorithm = HashAlgorithmName.SHA512;

    public static (string hashedPassword, string salt) HashPasword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(KeySize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            _hashAlgorithm,
            KeySize);

        return (Convert.ToHexString(hash), Convert.ToHexString(salt));
    }
    
    public static bool VerifyHashedPassword(string password, string hash, string salt)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromHexString(salt), Iterations, _hashAlgorithm, KeySize);

        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }

    public static string EncryptPassword(string password, string key, string iv)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = Encoding.UTF8.GetBytes(iv);

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(password);
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    public static string DecryptPassword(string encryptedPassword, string key, string iv)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.IV = Encoding.UTF8.GetBytes(iv);

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream(Convert.FromBase64String(encryptedPassword));
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        return sr.ReadToEnd();
    }
}