using System;
using System.IO;
using System.Net;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace TerminologyLauncher.Utils
{
    public static class DownloadUtils
    {
        public static readonly DirectoryInfo SystemTempFolder = new DirectoryInfo(Path.GetTempPath());
        public static void DownloadFile(String url, String path)
        {
            var tempFileInfo = new FileInfo(Path.Combine(SystemTempFolder.FullName, Guid.NewGuid().ToString("N")));

            using (var client = new WebClient())
            {
                client.DownloadFile(url, tempFileInfo.FullName);
                File.Copy(tempFileInfo.FullName, path, true);
            }
        }

        public static void DownloadFile(String url, String path, String md5)
        {
            DownloadFile(url, path);
            if (!Md5Utils.CheckFileMd5(path, md5))
            {
                throw new Exception(String.Format("Md5 check for {0} refused!", path));
            }
        }

        public static void DownloadAndExtractZippedFile(String url, String path, String md5)
        {
            var tempFileInfo = new FileInfo(Path.Combine(new[] { SystemTempFolder.FullName, Guid.NewGuid().ToString("N") }));
            DownloadFile(url, tempFileInfo.FullName, md5);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            new FastZip().ExtractZip(tempFileInfo.FullName, path, null);
        }

        public static void DownloadAndExtractZippedFile(String url, String path)
        {
            var tempFileInfo = new FileInfo(Path.Combine(new[] { SystemTempFolder.FullName, Guid.NewGuid().ToString("N") }));
            DownloadFile(url, tempFileInfo.FullName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            new FastZip().ExtractZip(tempFileInfo.FullName, path, null);
        }

        public static String GetFileContent(String url)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                return client.DownloadString(url);
            }
        }

    }
}
