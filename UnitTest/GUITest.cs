using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminologyLauncher.Configs;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.GUI.ToolkitWindows.SingleSelect;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.UnitTest
{
    [TestClass]
    public class GuiTest
    {
        [TestMethod]
        public void LoginWindowTest()
        {
            new GUI.LoginWindow(new Config(new FileInfo(""))).ShowDialog();
        }

        [TestMethod]
        public void MainWindowTest()
        {
            new GUI.MainWindow(new Config(new FileInfo(""))).ShowDialog();
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


        [TestMethod]
        public void SingleSelectTest()
        {
            var cancel = new SingleSelectWindow(null, "Test", "TestField", new List<String> { "asdasd", "dasdddd" }, new FieldReference<String>("dasdddd")).ShowDialog();
        }
    }
}
