using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.UnitTest
{
    [TestClass]
    public class ProgressUnitTests
    {
        [TestMethod]
        public void TopLevel()
        {
            var progress = new InternalNodeProgress();
            progress.ProgressChanged += sender =>
            {
                Console.WriteLine(progress.Percent);
            };
            Console.WriteLine(progress.Percent);
            this.MidLevel(progress.CreateNewInternalSubProgress(100));
            Console.WriteLine("All tasks finished!");
        }

        public void MidLevel(InternalNodeProgress progress)
        {
            this.BottomLevelA(progress.CreateNewLeafSubProgress(50));
            this.BottomLevelB(progress.CreateNewLeafSubProgress(50));
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
    }
}
