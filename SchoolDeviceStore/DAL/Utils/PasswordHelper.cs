using System;
using System.Security.Cryptography;

namespace DAL.Utils
{
    /// <summary>
    /// Simple PBKDF2 password helper. Generates salt and hash, verifies password.
    /// Suitable for .NET Framework 4.8 and SQL Server storage as varbinary.
    /// </summary>
    public static class PasswordHelper
    {
        private const int SaltSize = 16; // 128 bit
        private const int HashSize = 32; // 256 bit
        private const int Iterations = 10000; // adjustable

        public static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                hash = pbkdf2.GetBytes(HashSize);
            }
        }

        public static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, storedSalt, Iterations))
            {
                var computed = pbkdf2.GetBytes(HashSize);
                return AreEqual(computed, storedHash);
            }
        }

        private static bool AreEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            var diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
