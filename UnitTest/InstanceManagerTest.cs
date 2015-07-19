using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.UnitTest
{
    [TestClass]
    public class InstanceManagerTest
    {
        private static InstanceManager.InstanceManager InstanceManager { get; set; }

        [TestMethod]
        public void MainTest()
        {
            Directory.Delete("Instances", true);
            this.Initialize();
            this.LoadInstance();
            this.AddInstance();
            this.LaunchInstance();
            // this.RemoveInstance();
        }


        public void Initialize()
        {
            InstanceManager = new InstanceManager.InstanceManager();
            Assert.IsTrue(new DirectoryInfo("Instances").Exists);
        }

        public void LoadInstance()
        {
            InstanceManager.LoadInstancesFromInstanceFolder();
        }

        public void AddInstance()
        {
            InstanceManager.AddNewInstance("http://deckerchan.github.io/TerminologyResource/TestInstance.json");
            Assert.IsTrue(new DirectoryInfo("Instances").GetDirectories().Length != 0);
            Assert.IsTrue(new DirectoryInfo("Instances").GetDirectories()[0].GetFiles("*.png").Length == 2);
        }

        public void RemoveInstance()
        {
            InstanceManager.RemoveInstance(0);
            Assert.IsTrue(new DirectoryInfo("Instances").GetDirectories().Length == 0);
        }

        public void LaunchInstance()
        {
            var fileRepo = new FileRepository.FileRepository("Configs/FileRepositoryConfig.json");

            var progress = new InternalNodeProgress();
            progress.ProgressChanged += (i) =>
            {
                Console.WriteLine(progress.Percent);
            };
            InstanceManager.LaunchAnInstance(progress.CreateNewInternalSubProgress(100D), 0, fileRepo);
        }
    }
}
