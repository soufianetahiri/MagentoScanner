using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MagentoScanner.Helpers
{
    public static class HashCalculator
    {
        public static string CalculateMD5(byte[] fileBytes)
        {
#pragma warning disable SCS0006 // Weak hashing function
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
#pragma warning restore SCS0006 // Weak hashing function
            {
                using (MemoryStream ms = new MemoryStream(fileBytes))
                using (BufferedStream bs = new BufferedStream(ms, 100_000))
                {
                    return BitConverter.ToString(md5.ComputeHash(bs)).Replace("-", string.Empty);
                }
            }
        }
    }
}
