using System.Security.Cryptography;

namespace MyProject.Models
{
    public class PasswordHasher
    {
        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);
            return salt;
        }

        public static byte[] HashPassword(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);

            return pbkdf2.GetBytes(32);
        }

        public static bool Verify(string password, byte[] storedHash, byte[] storedSalt)
        {
            byte[] hash = HashPassword(password, storedSalt);

            return CryptographicOperations.FixedTimeEquals(hash, storedHash);
        }
    }
}
