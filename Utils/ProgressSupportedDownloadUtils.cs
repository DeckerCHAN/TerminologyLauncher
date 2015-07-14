using System;
using System.IO;
using System.Net;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Utils
{
    public static class ProgressSupportedDownloadUtils
    {
        public static string GetFileContent(LeafNodeProgress progress, string url)
        {
            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += (i, o) =>
                {
                    progress.Percent = o.ProgressPercentage;
                };

                return client.DownloadStringTaskAsync(url).Result;

            }
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
    }
}
