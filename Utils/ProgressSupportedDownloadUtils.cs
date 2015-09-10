using System;
using System.IO;
using System.Net;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Utils
{
    public static class ProgressSupportedDownloadUtils
    {
        public static string GetFileContent(LeafNodeProgress progress, string url)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.DownloadProgressChanged += (i, o) =>
                {
                    progress.Percent = o.ProgressPercentage;
                };

                return client.DownloadStringTaskAsync(url).Result;

            }
        }

        public static void DownloadZippedFile(InternalNodeProgress progress, String url, String path, String md5)
        {
            var tempFileInfo = new FileInfo(Path.Combine(new[] { DownloadUtils.SystemTempFolder.FullName, Guid.NewGuid().ToString("N") }));
            DownloadFile(progress.CreateNewLeafSubProgress(90D, "Downloading entire package."), url, tempFileInfo.FullName, md5);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            new FastZip().ExtractZip(tempFileInfo.FullName, path, null);
            progress.Percent = 100D;
        }

        public static void DownloadFile(LeafNodeProgress progress, String url, String path)
        {

            var tempFileInfo = new FileInfo(Path.Combine(DownloadUtils.SystemTempFolder.FullName, Guid.NewGuid().ToString("N")));
            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += (i, o) =>
                {
                    progress.Percent = o.ProgressPercentage;
                };
                client.DownloadFileTaskAsync(url, tempFileInfo.FullName).Wait();
                File.Copy(tempFileInfo.FullName, path);
            }
        }

        public static void DownloadFile(LeafNodeProgress progress, String url, String path, String md5)
        {
            DownloadFile(progress, url, path);
            if (!Md5Utils.CalculateFileMd5(path).ToUpper().Equals(md5.ToUpper()))
            {
                throw new Exception("Md5 not equivalent!");
            }
        }
    }
}
