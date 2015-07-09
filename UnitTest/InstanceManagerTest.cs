using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            
        }
    }
}
