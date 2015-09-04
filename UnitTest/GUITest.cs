using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.UnitTest
{
    [TestClass]
    public class GuiTest
    {
        [TestMethod]
        public void LoginWindowTest()
        {
            new GUI.LoginWindow().ShowDialog();
        }

        [TestMethod]
        public void MainWindowTest()
        {
            new GUI.MainWindow().ShowDialog();
        }

        [TestMethod]
        public void ProgressWindowTest()
        {
            var progress = new LeafNodeProgress("Common");
            var progressWindow = new GUI.ProgressWindow(progress);
           
            var t=new Task(() =>
            {
                progressWindow.ShowDialog();
            });
            t.Start();
            while (progress.Percent<100D)
            {
                Thread.Sleep(3000);
                progress.Percent += 1.5D;
            }
            t.Wait();
        }
    }
}
