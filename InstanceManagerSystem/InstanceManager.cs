using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.InstanceManagement.Local;
using TerminologyLauncher.Entities.InstanceManagement.Remote;
using TerminologyLauncher.Entities.SerializeUtils;
using TerminologyLauncher.FileRepositorySystem;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.InstanceManagerSystem
{
    public class InstanceManager
    {
        public Config Config { get; set; }
        public DirectoryInfo InstancesFolder { get; set; }
        public List<LocalInstanceEntity> Instances { get; set; }
        public Process CurrentInstanceProcess { get; set; }
        public FileRepository UsingFileRepository { get; protected set; }
        public InstanceManager(String configPath, FileRepository usingFileRepository)
        {
            this.Config = new Config(new FileInfo(configPath));
            this.UsingFileRepository = usingFileRepository;
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

        public void AddInstance(String instanceUrl)
        {
            //Download instance content
            var rowInstanceContent = DownloadUtils.GetFileContent(instanceUrl);
            var remoteInstance = JsonConverter.Parse<RemoteInstanceEntity>(rowInstanceContent);
            //Check instance already exists
            if (this.Instances.Any(x => (x.InstanceName.Equals(remoteInstance.InstanceName))))
            {
                throw new InvalidOperationException(String.Format("Instance {0} already exists!", remoteInstance.InstanceName));
            }


            //Localize instance
            var local = new LocalInstanceEntity();
            var thisInstanceFolder = new DirectoryInfo(Path.Combine(this.InstancesFolder.FullName, remoteInstance.InstanceName));
            if (thisInstanceFolder.Exists)
            {
                FolderUtils.DeleteDirectory(thisInstanceFolder.FullName);
            }
            thisInstanceFolder.Create();
            local.InstanceName = remoteInstance.InstanceName;
            local.Author = remoteInstance.Author;
            local.Description = remoteInstance.Description;
            local.FileSystem = remoteInstance.FileSystem;
            local.StartupArguments = remoteInstance.StartupArguments;
            local.Version = remoteInstance.Version;
            local.InstanceUpdateUrl = instanceUrl;
            //Download icon
            local.Icon = new FileInfo(Path.Combine(thisInstanceFolder.FullName, "icon.png")).FullName;
            try
            {
                DownloadUtils.DownloadFile(remoteInstance.Icon, local.Icon);
            }
            catch (WebException)
            {
                Logger.GetLogger().Warn("Can not download icon file.Using default instead.");
                ResourceUtils.CopyEmbedFileResource("TerminologyLauncher.InstanceManagerSystem.Resources.default_icon.png", new FileInfo(local.Icon));
            }
            //Download bg
            local.Background = new FileInfo(Path.Combine(thisInstanceFolder.FullName, "background.png")).FullName;
            try
            {
                DownloadUtils.DownloadFile(remoteInstance.Background, local.Background);

            }
            catch (WebException)
            {
                Logger.GetLogger().Warn("Can not download background file.Using default instead.");
                ResourceUtils.CopyEmbedFileResource("TerminologyLauncher.InstanceManagerSystem.Resources.default_bg.png", new FileInfo(local.Background));
            }
            //TODO:encrypt instance file if request(next version).
            var instanceFile = new FileInfo(Path.Combine(thisInstanceFolder.FullName, "Instance.json"));
            File.WriteAllText(instanceFile.FullName, JsonConverter.ConvertToJson(local));





            this.Instances.Add(local);
            Logger.GetLogger().Debug(String.Format("Added instance:{0}", remoteInstance.InstanceName));

        }

        public void RemoveInstance(Int32 index)
        {
            Directory.Delete(Path.Combine(this.InstancesFolder.FullName, this.Instances[index].InstanceName), true);

            var thisInstanceFolder = new DirectoryInfo(Path.Combine(this.InstancesFolder.FullName, this.Instances[index].InstanceName));
            if (thisInstanceFolder.Exists)
            {
                FolderUtils.DeleteDirectory(thisInstanceFolder.FullName);
            }
            this.Instances.RemoveAt(index);
            Logger.GetLogger().Debug(String.Format("Removed instance by index:{0}", index));
            this.LoadInstancesFromInstanceFolder();
        }

        public void UpdateInstance(Int32 index)
        {

        }

        public Process LaunchInstance(InternalNodeProgress progress, Int32 index)
        {
            var instance = this.Instances[index];
            var instanceRootFolder = this.GetInstanceRootFolder(instance.InstanceName);
            var placer = new PlaceHolderReplacer();
            placer.AddToDictionary("{root}", instanceRootFolder.FullName.Replace(" ", "\" \""));


            //Buding environment
            //Try to extract entire file.
            this.ReceiveEntirePackage(progress.CreateNewInternalSubProgress(30D), instance.InstanceName, instance.FileSystem.EntirePackageFile);

            //Try to check all official files.
            if (instance.FileSystem.OfficialFiles != null && instance.FileSystem.OfficialFiles.Count != 0)
            {
                var singleOfficialDownloadNodeProgress = 30D / instance.FileSystem.OfficialFiles.Count;
                foreach (var officialFileEntity in instance.FileSystem.OfficialFiles)
                {
                    this.ReceiveOfficialFile(progress.CreateNewLeafSubProgress(singleOfficialDownloadNodeProgress), instance.InstanceName, officialFileEntity, this.UsingFileRepository);
                }
            }
            progress.Percent = 60D;


            //Try to check all custom files.
            if (instance.FileSystem.CustomFiles != null && instance.FileSystem.CustomFiles.Count != 0)
            {
                var singleCustomDownloadNodeProgress = 30D / instance.FileSystem.CustomFiles.Count;
                foreach (var customFileEntity in instance.FileSystem.CustomFiles)
                {
                    this.ReceiveCustomFile(progress.CreateNewLeafSubProgress(singleCustomDownloadNodeProgress), instance.InstanceName, customFileEntity);
                }
            }

            progress.Percent = 90D;
            //TODO:Build start argument.
            var startArgument = placer.ReplaceArgument(instance.StartupArguments);

            progress.Percent = 100D;
            //Launch minecraft
            var instanceStartInfo = new ProcessStartInfo();
            var instanceProcess = new Process();
            instanceStartInfo.FileName = this.Config.GetConfig("javaPath");
            instanceStartInfo.Arguments = startArgument;
            instanceStartInfo.WorkingDirectory = instanceRootFolder.FullName;
            instanceStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            instanceStartInfo.UseShellExecute = false;
            instanceStartInfo.RedirectStandardOutput = true;
            instanceProcess.StartInfo = instanceStartInfo;
            instanceProcess.Start();

            this.CurrentInstanceProcess = instanceProcess;

            return this.CurrentInstanceProcess;


        }

        private DirectoryInfo GetInstanceRootFolder(String instanceName)
        {
            var folder = new DirectoryInfo(Path.Combine(this.InstancesFolder.FullName, instanceName));
            if (!folder.Exists)
            {
                throw new DirectoryNotFoundException(String.Format("Root folder for {0} not exists.", instanceName));
            }
            return folder;
        }

        private void ReceiveOfficialFile(LeafNodeProgress progress, String instanceName, OfficialFileEntity officialFile, FileRepository usingRepo)
        {
            //Try to find file at file repo 
            var officialRemoteFile = usingRepo.GetOfficialFile(officialFile.ProvideId);

            var downloadLink = officialRemoteFile.DownloadLink;
            var downloadTargetPositon = Path.Combine(this.GetInstanceRootFolder(instanceName).FullName, officialRemoteFile.LocalPath);

            ProgressSupportedDownloadUtils.DownloadFile(progress, downloadLink, downloadTargetPositon, officialRemoteFile.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} from remote url:{1}.", downloadTargetPositon, downloadLink));

        }

        private void ReceiveCustomFile(LeafNodeProgress progress, String instanceName, CustomFileEntity customFile)
        {
            var downloadLink = customFile.DownloadLink;
            var downloadTargetPositon = Path.Combine(this.GetInstanceRootFolder(instanceName).FullName, customFile.LocalPath);
            ProgressSupportedDownloadUtils.DownloadFile(progress, downloadLink, downloadTargetPositon, customFile.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} from remote url:{1}.", downloadTargetPositon, downloadLink));
        }

        private void ReceiveEntirePackage(InternalNodeProgress progress, String instanceName, EntirePackageFileEntity entirePackageFile)
        {
            var downloadLink = entirePackageFile.DownloadLink;
            var downloadTargetPositon = Path.Combine(this.GetInstanceRootFolder(instanceName).FullName, entirePackageFile.LocalPath ?? String.Empty);
            ProgressSupportedDownloadUtils.DownloadZippedFile(progress, downloadLink, downloadTargetPositon, entirePackageFile.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} then extracted to {1}.", downloadLink, downloadTargetPositon));
        }
    }
}
