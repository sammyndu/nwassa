using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nwassa.Core.Helpers
{
    public class Crypto : ICrypto
    {
        public string Hash(string text, string salt = null, int iterations = 1)
        {
            if (salt != null)
            {
                text += salt;
            }

            using (var sha = SHA256Managed.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                var hash = sha.ComputeHash(bytes);

                for (int i = 1; i < iterations; i++)
                {
                    hash = sha.ComputeHash(hash);
                }

                return ToHexString(hash);
            }
        }

        private static string ToHexString(byte[] byteArray)
        {
            var sb = new StringBuilder();

            foreach (var value in byteArray)
            {
                sb.Append(value.ToString("x2"));
            }

            return sb.ToString();
        }

        public string GenerateSalt(int maxLenght)
        {
            var salt = new byte[maxLenght];

            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

    }

    public interface ICrypto
    {
        string Hash(string text, string salt = null, int iterations = 1);

        string GenerateSalt(int maxLenght);
    }

}
