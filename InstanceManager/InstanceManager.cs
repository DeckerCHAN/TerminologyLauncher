using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.InstanceManagement.Local;
using TerminologyLauncher.Entities.InstanceManagement.Remote;
using TerminologyLauncher.Entities.SerializeUtils;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.InstanceManager
{
    public class InstanceManager
    {
        public Config Config { get; set; }
        public DirectoryInfo InstancesFolder { get; set; }
        public List<LocalInstanceEntity> Instances { get; set; }
        public Process CurrentInstanceProcess { get; set; }
        public InstanceManager()
        {
            this.Config = new Config(new FileInfo("Configs/InstanceManagerConfig.json"));
            this.InstancesFolder = new DirectoryInfo(this.Config.GetConfig("instancesFolderPath"));
            this.Instances = new List<LocalInstanceEntity>();

            if (!this.InstancesFolder.Exists)
            {
                this.InstancesFolder.Create();
            }
        }

        public void LoadInstancesFromInstanceFolder()
        {
            this.Instances = new List<LocalInstanceEntity>();
            var folders = this.InstancesFolder.GetDirectories();
            foreach (var singleInstanceFolder in folders)
            {
                String instanceFileContent = null;

                var commonInstanceFile = new FileInfo(Path.Combine(singleInstanceFolder.FullName, "Instance.json"));
                var encryptedInstanceFile = new FileInfo(Path.Combine(singleInstanceFolder.FullName, "Instance.bin"));

                if (commonInstanceFile.Exists)
                {
                    instanceFileContent = File.ReadAllText(commonInstanceFile.FullName);
                }
                else if (encryptedInstanceFile.Exists)
                {
                    throw new NotImplementedException("Support encrypted File");
                    instanceFileContent = File.ReadAllText(commonInstanceFile.FullName);
                }
                else
                {
                    //This is not an instance folder, ignore this.
                    continue;
                }

                //Then add this instance to instance collection.
                try
                {
                    this.Instances.Add(JsonConverter.Parse<LocalInstanceEntity>(instanceFileContent));
                }
                catch (Exception)
                {
                    Logger.GetLogger().Warn(String.Format("Can not convert instance file in {0} folder. Ignore this.",
                         singleInstanceFolder.Name));
                    continue;
                }

            }
        }

        public void AddNewInstance(String instanceUrl)
        {
            var rowInstanceContent = DownloadUtils.GetFileContent(instanceUrl);
            var instance = JsonConverter.Parse<RemoteInstanceEntity>(rowInstanceContent);
            var localInstance = this.RemoteEntityLocalize(instance, instanceUrl);
            //this.Instances.Add(localInstance);
            Logger.GetLogger().Debug(String.Format("Added instance:{0}", instance.InstanceName));
            this.LoadInstancesFromInstanceFolder();
        }

        public void RemoveInstance(Int32 index)
        {
            Directory.Delete(Path.Combine(this.InstancesFolder.FullName, this.Instances[index].InstanceName), true);
            this.Instances.RemoveAt(index);
            Logger.GetLogger().Debug(String.Format("Removed instance by index:{0}", index));
            this.LoadInstancesFromInstanceFolder();
        }

        private LocalInstanceEntity RemoteEntityLocalize(RemoteInstanceEntity remote, String instanceUrl)
        {
            var local = new LocalInstanceEntity();
            var thisInstanceFolder = new DirectoryInfo(Path.Combine(this.InstancesFolder.FullName, remote.InstanceName));
            if (thisInstanceFolder.Exists)
            {
                thisInstanceFolder.Delete(true);
            }
            thisInstanceFolder.Create();
            local.InstanceName = remote.InstanceName;
            local.Author = remote.Author;
            local.Description = remote.Description;
            local.FileSystem = remote.FileSystem;
            local.StartupScript = remote.StartupScript;
            local.Version = remote.Version;
            local.InstanceUpdateUrl = instanceUrl;
            //Download icon
            local.Icon = new FileInfo(Path.Combine(thisInstanceFolder.FullName, "icon.png")).FullName;
            try
            {
                DownloadUtils.DownloadFile(remote.Icon, local.Icon);
            }
            catch (WebException)
            {
                Logger.GetLogger().Warn("Can not download icon file.Using default instead.");
                ResourceUtils.CopyEmbedFileResource("TerminologyLauncher.InstanceManager.Resources.default_icon.png", new FileInfo(local.Icon));
            }
            //Download bg
            local.Background = new FileInfo(Path.Combine(thisInstanceFolder.FullName, "background.png")).FullName;
            try
            {
                DownloadUtils.DownloadFile(remote.Background, local.Background);

            }
            catch (WebException)
            {
                Logger.GetLogger().Warn("Can not download background file.Using default instead.");
                ResourceUtils.CopyEmbedFileResource("TerminologyLauncher.InstanceManager.Resources.default_bg.png", new FileInfo(local.Background));
            }
            //TODO:encrypt instance file if request.
            var instanceFile = new FileInfo(Path.Combine(thisInstanceFolder.FullName, "Instance.json"));
            File.WriteAllText(instanceFile.FullName, JsonConverter.ConvertToJson(local));
            return local;
        }

        public Process LaunchAnInstance(Int32 index, FileRepository.FileRepository usingFileRepository)
        {
            var instance = this.Instances[index];
            var rootFolder = this.GetInstanceRootFolder(instance.InstanceName);
            //Buding environment
            usingFileRepository.ReceiveEntirePackage(rootFolder, instance.FileSystem.EntirePackageFile);
            foreach (var officialFileEntity in instance.FileSystem.OfficialFiles)
            {
                usingFileRepository.ReceiveOfficialFile(rootFolder, officialFileEntity);
            }
            foreach (var customFileEntity in instance.FileSystem.CustomFiles)
            {
                usingFileRepository.ReceiveCustomFile(rootFolder, customFileEntity);
            }

            //Launch minecraft
            var instanceStartInfo = new ProcessStartInfo();
            var instanceProcess = new Process();
            instanceStartInfo.FileName = "java.exe";
            instanceStartInfo.WindowStyle = ProcessWindowStyle.Normal;


            return this.CurrentInstanceProcess;

        }

        public DirectoryInfo GetInstanceRootFolder(String instanceName)
        {
            var folder = new DirectoryInfo(Path.Combine(this.InstancesFolder.FullName, instanceName));
            if (!folder.Exists)
            {
                throw new DirectoryNotFoundException();
            }
            return folder;
        }
    }
}
