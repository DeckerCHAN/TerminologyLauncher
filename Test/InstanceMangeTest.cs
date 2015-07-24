using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminologyLauncher.FileRepositorySystem;
using TerminologyLauncher.InstanceManagerSystem;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.ProgressService;

namespace Test
{
    public class InstanceMangeTest
    {
        private static InstanceManager InstanceManager { get; set; }

        public static void MainTest()
        {
            if (Directory.Exists("Instances"))
            {
                FolderUtils.DeleteDirectory("Instances");
            }
            Initialize();
            LoadInstance();
            AddInstance();
            LaunchInstance();
            // this.RemoveInstance();
        }


        public static void Initialize()
        {
            var fileRepo = new FileRepository("Configs/FileRepositoryConfig.json");
            InstanceManager = new InstanceManager("Configs/InstanceManagerConfig.json", fileRepo);
            Assert.IsTrue(new DirectoryInfo("Instances").Exists);
        }

        public static void LoadInstance()
        {
            InstanceManager.LoadInstancesFromInstanceFolder();
        }

        public static void AddInstance()
        {
            InstanceManager.AddNewInstance("http://192.168.1.103/PureMC.json");
            Assert.IsTrue(new DirectoryInfo("Instances").GetDirectories().Length != 0);
            Assert.IsTrue(new DirectoryInfo("Instances").GetDirectories()[0].GetFiles("*.png").Length == 2);
        }

        public static void RemoveInstance()
        {
            InstanceManager.RemoveInstance(0);
            Assert.IsTrue(new DirectoryInfo("Instances").GetDirectories().Length == 0);
        }

        public static void LaunchInstance()
        {
            if (InstanceManager.Config != null)
                InstanceManager.Config.SetConfig("javaPath", "C:\\jdk1.7.0_51\\bin\\java.exe");
            var fileRepo = new FileRepository("Configs/FileRepositoryConfig.json");

            var progress = new InternalNodeProgress();
            progress.ProgressChanged += (i) =>
            {
                Console.WriteLine(progress.Percent);
            };
            var process = InstanceManager.LaunchAnInstance(progress.CreateNewInternalSubProgress(100D), 0);
            process.WaitForExit();
        }
    }
}
