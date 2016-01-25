using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Utils
{
    public static class DownloadUtils
    {
        public static void DownloadFile(String url, String path)
        {
            var tempFileInfo = new FileInfo(Path.Combine(FolderUtils.SystemTempFolder.FullName, Guid.NewGuid().ToString("N")));
            var targetFileInfo = new FileInfo(path);
            using (var client = new WebClient())
            {
                client.DownloadFile(url, tempFileInfo.FullName);
                if (targetFileInfo.Directory != null && !targetFileInfo.Directory.Exists)
                {
                    targetFileInfo.Directory.Create();
                }
                File.Copy(tempFileInfo.FullName, targetFileInfo.FullName, true);
            }
        }

        public static void DownloadFile(String url, String path, String md5)
        {
            DownloadFile(url, path);
            if (!EncodeUtils.CheckFileMd5(path, md5))
            {
                throw new Exception(String.Format("Md5 check for {0} refused!", path));
            }
        }

        public static void DownloadFile(LeafNodeProgress progress, String url, String path)
        {
            var task = new Task(() =>
            {
                var tempFileInfo = new FileInfo(Path.Combine(FolderUtils.SystemTempFolder.FullName, Guid.NewGuid().ToString("N")));
                var targetFileInfo = new FileInfo(path);
                using (var client = new WebClient())
                {
                    client.DownloadProgressChanged += (i, o) =>
                    {
                        progress.Percent = o.ProgressPercentage;
                    };
                    try
                    {
                        client.DownloadFileTaskAsync(url, tempFileInfo.FullName).Wait();
                    }
                    catch (AggregateException ex)
                    {
                        throw ex.GetBaseException();
                    }
                    catch (ArgumentException ex)
                    {
                        throw new ArgumentException(ex.Message + String.Format("Url:{0}", url), ex);
                    }

                    if (targetFileInfo.Directory != null && !targetFileInfo.Directory.Exists)
                    {
                        targetFileInfo.Directory.Create();
                    }
                    File.Copy(tempFileInfo.FullName, targetFileInfo.FullName, true);
                }
            });
            task.Start();
            try
            {
                task.Wait();
            }
            catch (AggregateException ae)
            {

                throw ae.InnerException;
            }


        }

        public static void DownloadFile(LeafNodeProgress progress, String url, String path, String md5)
        {
            DownloadFile(progress, url, path);
            if (!EncodeUtils.CheckFileMd5(path, md5))
            {
                throw new Exception(String.Format("Md5 check for {0} refused!", path));
            }
        }

        public static void DownloadAndExtractZippedFile(String url, String path, String md5)
        {
            var tempFileInfo = new FileInfo(Path.Combine(new[] { FolderUtils.SystemTempFolder.FullName, Guid.NewGuid().ToString("N") }));
            DownloadFile(url, tempFileInfo.FullName, md5);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            new FastZip().ExtractZip(tempFileInfo.FullName, path, null);
        }

        public static void DownloadAndExtractZippedFile(String url, String path)
        {
            var tempFileInfo = new FileInfo(Path.Combine(new[] { FolderUtils.SystemTempFolder.FullName, Guid.NewGuid().ToString("N") }));
            DownloadFile(url, tempFileInfo.FullName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            new FastZip().ExtractZip(tempFileInfo.FullName, path, null);
        }

        public static String GetWebContent(String url)
        {
            using (var client = new WebClient())
            {
                client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                client.Encoding = EncodeUtils.NoneBomUTF8;
                return client.DownloadString(url);
            }
        }

        [STAThread]
        public static String GetWebContent(LeafNodeProgress progress, string url)
        {
            var task = new Task<String>(() =>
            {
                using (var client = new WebClient())
                {
                    client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    client.Encoding = EncodeUtils.NoneBomUTF8;
                    client.DownloadProgressChanged += (i, o) =>
                    {
                        progress.Percent = o.ProgressPercentage;
                    };
                    try
                    {
                        return client.DownloadStringTaskAsync(url).Result;
                    }
                    catch (ArgumentException ex)
                    {
                        throw new ArgumentException(ex.Message + String.Format("Url:{0}", url), ex);
                    }
                }
            });
            task.Start();
            task.Wait();
            return task.Result;
        }

        public static void DownloadZippedFile(InternalNodeProgress progress, String url, String path, String md5)
        {
            var tempFileInfo = new FileInfo(Path.Combine(new[] { FolderUtils.SystemTempFolder.FullName, Guid.NewGuid().ToString("N") }));
            DownloadFile(progress.CreateNewLeafSubProgress(String.Format("Downloading and unzipping zip file {0}", url), 90D), url, tempFileInfo.FullName, md5);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            new FastZip().ExtractZip(tempFileInfo.FullName, path, null);
            progress.Percent = 100D;
        }
        public static void DownloadZippedFile(String url, String path, String md5)
        {
            var tempFileInfo = new FileInfo(Path.Combine(new[] { FolderUtils.SystemTempFolder.FullName, Guid.NewGuid().ToString("N") }));
            DownloadFile(url, tempFileInfo.FullName, md5);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            new FastZip().ExtractZip(tempFileInfo.FullName, path, null);
        }

    }
}
