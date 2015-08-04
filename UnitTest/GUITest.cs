using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var progressWindow = new GUI.ProgressWindow();
            progressWindow.Show();

            while (true)
            {
                Thread.Sleep(3000);
                progressWindow.Progress += 15D;
            }
        }
    }
}
