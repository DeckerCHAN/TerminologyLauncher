using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TerminologyLauncher.Utils
{
    public static class Md5Utils
    {
        public static String CalculateFileMd5(String fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return ToHex(md5.ComputeHash(stream), true);
                }
            }
        }
        public static String ToHex(this byte[] bytes, bool upperCase)
        {
            var result = new StringBuilder(bytes.Length * 2);

            foreach (var t in bytes)
                result.Append(t.ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }
    }


}
