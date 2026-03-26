using System.Security.Cryptography;
using System.Text;
using TheHouseBebidas.WineReviews.Application.Interfaces.Auth;

namespace TheHouseBebidas.WineReviews.Infrastructure.Auth;

public sealed class PasswordHasher : IPasswordHasher
{
    public (string Hash, string Salt) HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password is required.", nameof(password));
        }

        using var hmac = new HMACSHA512();
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        var saltBytes = hmac.Key;

        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    public bool VerifyPassword(string password, string passwordHash, string passwordSalt)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(passwordHash) ||
            string.IsNullOrWhiteSpace(passwordSalt))
        {
            return false;
        }

        byte[] saltBytes;
        byte[] expectedHashBytes;

        try
        {
            saltBytes = Convert.FromBase64String(passwordSalt);
            expectedHashBytes = Convert.FromBase64String(passwordHash);
        }
        catch (FormatException)
        {
            return false;
        }

        using var hmac = new HMACSHA512(saltBytes);
        var actualHashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return CryptographicOperations.FixedTimeEquals(actualHashBytes, expectedHashBytes);
    }
}
