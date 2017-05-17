using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.UnitTest
{
    [TestClass]
    public class ProgressUnitTests
    {
        [TestMethod]
        public void TopLevel()
        {
            var progress = new InternalNodeProgress("Main task");
            progress.ProgressChanged +=
                sender => { Console.WriteLine($"Woring on {progress.TaskName}:{progress.Percent}"); };
            Console.WriteLine(progress.Percent);
            this.MidLevel(progress.CreateNewInternalSubProgress("MidLevel", 100D));
            Console.WriteLine("All tasks finished!");
        }

        public void MidLevel(InternalNodeProgress progress)
        {
            this.BottomLevelA(progress.CreateNewLeafSubProgress("MidLevel A", 50D));
            this.BottomLevelB(progress.CreateNewLeafSubProgress("MidLevel B", 50));
        }

        public void BottomLevelA(LeafNodeProgress progress)
        {
            Thread.Sleep(2000);
            progress.Percent = 20;
            Thread.Sleep(2000);
            progress.Percent = 40;
            Thread.Sleep(2000);
            progress.Percent = 60;
            Thread.Sleep(2000);
            progress.Percent = 80;
            Thread.Sleep(2000);
            progress.Percent = 100;
        }

        public void BottomLevelB(LeafNodeProgress progress)
        {
            progress.Percent = 0;
            Thread.Sleep(2000);
            progress.Percent = 20;
            Thread.Sleep(2000);
            progress.Percent = 40;
            Thread.Sleep(2000);
            progress.Percent = 60;
            Thread.Sleep(2000);
            progress.Percent = 80;
            Thread.Sleep(2000);
            progress.Percent = 100;
        }

        [TestMethod]
        [MTAThread]
        public void DownloadText()
        {
            var progress = new InternalNodeProgress("Download text main task");
            progress.ProgressChanged += sender => { Console.WriteLine(progress.Percent); };
            var content =
                DownloadUtils.GetWebContent(progress.CreateNewLeafSubProgress(string.Format("Downloading Text"), 100D),
                    "http://baidu.com");
            // Console.WriteLine(content);
        }

        [TestMethod]
        public void DownloadFileWithProgress()
        {
            var progress = new InternalNodeProgress("Download File Main Task");
            progress.ProgressChanged += sender => { Console.WriteLine(progress.Percent); };
            var downloadFile = new FileInfo("Test.QQ.exe");
            if (downloadFile.Exists)
            {
                downloadFile.Delete();
            }
            DownloadUtils.DownloadFile(progress.CreateNewLeafSubProgress("Downloading test file", 100D),
                "http://dldir1.qq.com/qqfile/qq/QQ7.4/15197/QQ7.4.exe", downloadFile.FullName);
            downloadFile.Refresh();
            Assert.IsTrue(downloadFile.Exists);
        }
    }
}