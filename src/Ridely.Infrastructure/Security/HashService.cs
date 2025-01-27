using System.Security.Cryptography;
using System.Text;
using Ridely.Application.Abstractions.Security;

namespace Ridely.Infrastructure.Security;
internal sealed class HashService : IHashService
{
    public string GenerateSalt()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(bytes);
    }

    public string HashAndSaltValue(string value)
    {
        string salt = GenerateSalt();

        byte[] saltedPassword = Encoding.UTF8.GetBytes(salt + value);

        byte[] hash = SHA256.HashData(saltedPassword);

        return $"{Convert.ToBase64String(hash)}:{salt}";
    }

    public string HashAndSaltValue(string value, string salt)
    {
        byte[] saltedPassword = Encoding.UTF8.GetBytes(salt + value);

        byte[] hash = SHA256.HashData(saltedPassword);

        return $"{Convert.ToBase64String(hash)}:{salt}";
    }

    public bool ComparePassword(string password, string value)
    {
        string[] hashedArr = password.Split(":");

        if (hashedArr.Length != 2) return false;

        string salt = hashedArr[1];

        string newValueHashedWithSalt = HashAndSaltValue(value, salt);

        return password.Equals(newValueHashedWithSalt);
    }

    public string HashValue(string value)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));

        return Convert.ToBase64String(hash);
    }
}
