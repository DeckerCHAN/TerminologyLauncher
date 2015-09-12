using System;
using System.IO;
using System.Net;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Utils
{
    public static class DownloadUtils
    {
        public static void DownloadFile(String url, String path)
        {
            var tempFileInfo = new FileInfo(Path.Combine(FolderUtils.SystemTempFolder.FullName, Guid.NewGuid().ToString("N")));
            var targetFolderInfo = new DirectoryInfo(path);
            using (var client = new WebClient())
            {
                client.DownloadFile(url, tempFileInfo.FullName);
                if (!targetFolderInfo.Exists)
                {
                    targetFolderInfo.Create();
                }
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

        public static void DownloadFile(LeafNodeProgress progress, String url, String path)
        {

            var tempFileInfo = new FileInfo(Path.Combine(FolderUtils.SystemTempFolder.FullName, Guid.NewGuid().ToString("N")));
            var targetFolderInfo = new DirectoryInfo(path);
            using (var client = new WebClient())
            {
                client.DownloadProgressChanged += (i, o) =>
                {
                    progress.Percent = o.ProgressPercentage;
                };
                client.DownloadFileTaskAsync(url, tempFileInfo.FullName).Wait();
                if (!targetFolderInfo.Exists)
                {
                    targetFolderInfo.Create();
                }
                File.Copy(tempFileInfo.FullName, targetFolderInfo.FullName, true);
            }
        }

        public static void DownloadFile(LeafNodeProgress progress, String url, String path, String md5)
        {
            DownloadFile(progress, url, path);
            if (!Md5Utils.CheckFileMd5(path, md5))
            {
                throw new Exception(String.Format("Md5 check for {0} refused!", path));
            }
        }

        public static void DownloadAndExtractZippedFile(String url, String path, String md5)
        {
            var tempFileInfo = new FileInfo(Path.Combine(new[] {FolderUtils.SystemTempFolder.FullName, Guid.NewGuid().ToString("N") }));
            DownloadFile(url, tempFileInfo.FullName, md5);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            new FastZip().ExtractZip(tempFileInfo.FullName, path, null);
        }

        public static void DownloadAndExtractZippedFile(String url, String path)
        {
            var tempFileInfo = new FileInfo(Path.Combine(new[] {FolderUtils.SystemTempFolder.FullName, Guid.NewGuid().ToString("N") }));
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
            var tempFileInfo = new FileInfo(Path.Combine(new[] { FolderUtils.SystemTempFolder.FullName, Guid.NewGuid().ToString("N") }));
            DownloadFile(progress.CreateNewLeafSubProgress(90D, String.Format("Downloading zip file {0}", url)), url, tempFileInfo.FullName, md5);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            new FastZip().ExtractZip(tempFileInfo.FullName, path, null);
            progress.Percent = 100D;
        }

    }
}
