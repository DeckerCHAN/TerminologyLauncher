using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.FileRepositorySystem;
using TerminologyLauncher.InstanceManagerSystem;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Test
{
    public class InstanceMangeTest
    {
        private static InstanceManager InstanceManager { get; set; }

        public static void MainTest()
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadKey();
            }


        }


        public static void Initialize()
        {
            var fileRepo = new FileRepository("Configs/FileRepositoryConfig.json");
            InstanceManager = new InstanceManager("Configs/InstanceManagerConfig.json", fileRepo);
            Assert.IsTrue(new DirectoryInfo("Instances").Exists);
        }

        public static void LoadInstance()
        {
            InstanceManager.LoadInstancesFromBankFile();
        }

        public static void AddInstance()
        {
            InstanceManager.AddInstance("http://terminology.b0.upaiyun.com/PureMC.json");
            Assert.IsTrue(new DirectoryInfo("Instances").GetDirectories().Length != 0);
            Assert.IsTrue(new DirectoryInfo("Instances").GetDirectories()[0].GetFiles("*.png").Length == 2);
        }

        public static void RemoveInstance()
        {
            InstanceManager.RemoveInstance(InstanceManager.Instances[0].InstanceName);
            Assert.IsTrue(new DirectoryInfo("Instances").GetDirectories().Length == 0);
        }

        public static void LaunchInstance()
        {
            if (InstanceManager.Config != null)
                InstanceManager.Config.SetConfigString("javaBinPath", "C:\\jdk1.7.0_51\\bin\\");
            var fileRepo = new FileRepository("Configs/FileRepositoryConfig.json");

            var progress = new InternalNodeProgress("");
            var t = Task.Run(() =>
            {
                while (progress.Percent <= 100D)
                {
                    Console.WriteLine(progress.Percent);
                    Thread.Sleep(1000);
                }

            });
            var process = InstanceManager.LaunchInstance(progress.CreateNewInternalSubProgress(100D, ""), InstanceManager.Instances[0].InstanceName, new PlayerEntity() { PlayerName = "DeckerCHAN" });
            process.WaitForExit();
        }
    }
}
