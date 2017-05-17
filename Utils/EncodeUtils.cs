using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TerminologyLauncher.Utils
{
    public static class EncodeUtils
    {
        public static Encoding NoneBomUTF8 => new UTF8Encoding(false);

        public static long CalcuateFileSize(string filePath)
        {
          return  new FileInfo(filePath).Length;
        }

        public static string CalculateFileMd5(string filePath)
        {
            if (!File.Exists(filePath) || File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
            {
                throw new FileNotFoundException(
                    $"Cannot calculate file Md5 for {filePath} if file not exists or it is a directory!");
            }
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    return ToHex(md5.ComputeHash(stream), true);
                }
            }
        }

        public static string CalculateStringMd5(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new Exception("Empty string is not allowed!");
            }
            using (var md5 = MD5.Create())
            {
                return ToHex(md5.ComputeHash(Encoding.Default.GetBytes(content)), true);
            }
        }

        public static string ToHex(this byte[] bytes, bool upperCase)
        {
            var result = new StringBuilder(bytes.Length*2);

            foreach (var t in bytes)
                result.Append(t.ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

        public static bool CheckFileMd5(string filePath, string md5)
        {
            return CalculateFileMd5(filePath).ToUpper().Equals(md5.ToUpper());
        }

        public static bool CheckFileSize(string filePath, long size)
        {

            return CalcuateFileSize(filePath).Equals(size);
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}