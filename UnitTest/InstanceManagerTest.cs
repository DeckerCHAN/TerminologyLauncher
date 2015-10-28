using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.FileRepositorySystem;
using TerminologyLauncher.InstanceManagerSystem;
using TerminologyLauncher.JreManagerSystem;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.UnitTest
{
    [TestClass]
    public class InstanceManagerTest
    {
        private static InstanceManager InstanceManager { get; set; }

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
            var fileRepo = new FileRepository("Configs/FileRepositoryConfig.json");
            InstanceManager = new InstanceManager("Configs/InstanceManagerConfig.json", fileRepo,new JreManager(""));
            Assert.IsTrue(new DirectoryInfo("Instances").Exists);
        }

        public void LoadInstance()
        {
            InstanceManager.LoadInstancesFromBankFile();
        }

        public void AddInstance()
        {
            InstanceManager.AddInstance("http://terminology.b0.upaiyun.com/PureMC.json");
            Assert.IsTrue(new DirectoryInfo("Instances").GetDirectories().Length != 0);
            Assert.IsTrue(new DirectoryInfo("Instances").GetDirectories()[0].GetFiles("*.png").Length == 2);
        }

        public void RemoveInstance()
        {
            InstanceManager.RemoveInstance(InstanceManager.Instances[0].InstanceName);
            Assert.IsTrue(new DirectoryInfo("Instances").GetDirectories().Length == 0);
        }

        public void LaunchInstance()
        {
            if (InstanceManager.Config != null)
                InstanceManager.Config.SetConfigString("javaBinPath", "C:\\jdk1.7.0_51\\bin\\");


            var progress = new InternalNodeProgress("");
            progress.ProgressChanged += (i) =>
            {
                Console.WriteLine(progress.Percent);
            };
            InstanceManager.LaunchInstance(progress.CreateNewInternalSubProgress("", 100D), InstanceManager.Instances[0].InstanceName, new PlayerEntity() { PlayerName = "DeckerCHAN" });
        }
    }
}
