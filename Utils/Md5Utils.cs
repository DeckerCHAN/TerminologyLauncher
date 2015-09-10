using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TerminologyLauncher.Utils
{
    public static class Md5Utils
    {
        public static String CalculateFileMd5(String filePath)
        {
            if (!File.Exists(filePath) || File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
            {
                throw new FileNotFoundException(
                    String.Format("Can not calculate file Md5 for {0} if file not exists or it is a directory!", filePath));
            }
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
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

        public static Boolean CheckFileMd5(String filePath, String md5)
        {
            return CalculateFileMd5(filePath).ToUpper().Equals(md5.ToUpper());
        }
    }


}
