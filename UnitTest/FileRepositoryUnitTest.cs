using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.UnitTest
{
    [TestClass]
    public class FileRepositoryUnitTest
    {
        [TestMethod]
        public void CommonDownloadTest()
        {
            var downloadFile = new FileInfo("QQ.exe");
            if (downloadFile.Exists)
            {
                downloadFile.Delete();
            }
            DownloadUtils.DownloadFile(
                "http://59.67.148.46/files/506400000010EC32/dldir1.qq.com/qqfile/qq/QQ7.3/15056/QQ7.3.exe", downloadFile.FullName);
            Assert.IsTrue(downloadFile.Exists);
        }

        [TestMethod]
        public void Md5Test()
        {
            var downloadFile = new FileInfo("QQ.exe");
            var md5 = EncodeUtils.CalculateFileMd5(downloadFile.FullName);
            Console.WriteLine(md5);
            Assert.IsNotNull(md5);
        }

        [TestMethod]
        public void DownloadZipTest()
        {
            DownloadUtils.DownloadAndExtractZippedFile("http://www.colorado.edu/conflict/peace/download/peace.zip",
                Path.Combine(new[] { Environment.CurrentDirectory, Path.Combine("ZIPFOLDER","Zepped") }));
            Assert.IsTrue(new DirectoryInfo(Path.Combine(new[] { Environment.CurrentDirectory, "ZIPFOLDER" })).Exists);
        }

        [TestMethod]
        public void DownloadStringTest()
        {
            var content = DownloadUtils.GetFileContent("http://www.baidu.com");
            Assert.IsFalse(String.IsNullOrEmpty(content));
        }
    }
}
