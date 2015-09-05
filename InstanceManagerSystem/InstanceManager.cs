using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.InstanceManagement.FileSystem;
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
        public InstanceBankEntity InstanceBank { get; set; }
        public Process CurrentInstanceProcess { get; set; }
        public FileRepository UsingFileRepository { get; protected set; }
        public InstanceManager(String configPath, FileRepository usingFileRepository)
        {
            this.Config = new Config(new FileInfo(configPath));
            this.UsingFileRepository = usingFileRepository;
            this.InstancesFolder = new DirectoryInfo(this.Config.GetConfig("instancesFolderPath"));
            if (!this.InstancesFolder.Exists)
            {
                this.InstancesFolder.Create();
            }
            this.LoadInstancesFromBankFile();
        }

        public List<InstanceEntity> Instances
        {
            get
            {
                return this.InstanceBank.InstancesInfoList.Select(instanceInfo => JsonConverter.Parse<InstanceEntity>(File.ReadAllText(instanceInfo.FilePath))).ToList();
            }
        }

        public List<InstanceEntity> InstancesWithLocalImageSource
        {
            get
            {
                var instances = this.Instances;
                foreach (var instanceEntity in instances)
                {
                    instanceEntity.Icon = this.GetIconImage(instanceEntity.InstanceName);
                    instanceEntity.Background = this.GetBackgroundImage(instanceEntity.InstanceName);
                }
                return instances;
            }
        }

        public InstanceEntity[] InstancesArray
        {
            get { return this.Instances.ToArray(); }
        }

        public void LoadInstancesFromBankFile()
        {

            if (File.Exists(this.Config.GetConfig("instanceBankFilePath")))
            {
                this.InstanceBank = JsonConverter.Parse<InstanceBankEntity>(File.ReadAllText(this.Config.GetConfig("instanceBankFilePath")));
            }
            else
            {
                this.InstanceBank = new InstanceBankEntity
                {
                    InstancesInfoList = new List<InstanceInfoEntity>()
                };
            }
        }

        public void SaveInstancesToBankFile()
        {
            var content = JsonConverter.ConvertToJson(this.InstanceBank);
            File.WriteAllText(this.Config.GetConfig("instanceBankFilePath"), content);
        }

        public void AddInstance(String instanceUrl)
        {
            Logger.GetLogger().Info(String.Format("Starting to add new instance through {0}.", instanceUrl));
            this.LoadInstancesFromBankFile();
            //Download instance content
            var rowInstanceContent = DownloadUtils.GetFileContent(instanceUrl);

            var instance = JsonConverter.Parse<InstanceEntity>(rowInstanceContent);

            if (String.IsNullOrEmpty(instance.InstanceName))
            {
                Logger.GetLogger().Error("Missing instance name");
                throw new MissingFieldException();
            }

            if (String.IsNullOrEmpty(instance.UpdatePath))
            {
                Logger.GetLogger().Error("Missing instance update path");
                throw new MissingFieldException();
            }

            var instanceInfo = new InstanceInfoEntity();

            //Check instance already exists
            if (this.Instances.Any(x => (x.InstanceName.Equals(instance.InstanceName))))
            {
                throw new InvalidOperationException(String.Format("Instance {0} already exists!", instance.InstanceName));
            }


            //Localize instance
            var thisInstanceFolder = new DirectoryInfo(Path.Combine(this.InstancesFolder.FullName, instance.InstanceName));
            if (thisInstanceFolder.Exists)
            {
                FolderUtils.DeleteDirectory(thisInstanceFolder.FullName);
            }
            thisInstanceFolder.Create();

            instanceInfo.Name = instance.InstanceName;
            instanceInfo.InstanceState = InstanceState.Initialize;
            instanceInfo.FilePath = Path.Combine(this.GetInstanceRootFolder(instance.InstanceName).FullName,
                "Instance.json");
            instanceInfo.UpdateUrl = instance.UpdatePath;
            instanceInfo.UpdateDate = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            //Download icon
            var iconFile = new FileInfo(Path.Combine(thisInstanceFolder.FullName, "icon.png")).FullName;
            try
            {
                DownloadUtils.DownloadFile(instance.Icon, iconFile);
            }
            catch (WebException)
            {
                Logger.GetLogger().Warn("Can not download icon file.Using default instead.");
                ResourceUtils.CopyEmbedFileResource(
                    "TerminologyLauncher.InstanceManagerSystem.Resources.default_icon.png", new FileInfo(iconFile));
            }
            //Download bg
            var bgFile = new FileInfo(Path.Combine(thisInstanceFolder.FullName, "background.png")).FullName;
            try
            {
                DownloadUtils.DownloadFile(instance.Background, bgFile);

            }
            catch (WebException)
            {
                Logger.GetLogger().Warn("Can not download background file.Using default instead.");
                ResourceUtils.CopyEmbedFileResource(
                    "TerminologyLauncher.InstanceManagerSystem.Resources.default_bg.png", new FileInfo(bgFile));
            }
            //TODO:encrypt instance file if request(next version).
            File.WriteAllText(instanceInfo.FilePath, JsonConverter.ConvertToJson(instance));
            this.InstanceBank.InstancesInfoList.Add(instanceInfo);
            this.SaveInstancesToBankFile();
            Logger.GetLogger().Debug(String.Format("Added instance:{0}", instance.InstanceName));

        }

        public void RemoveInstance(String instanceName)
        {
            this.LoadInstancesFromBankFile();

            var thisInstanceFolder = new DirectoryInfo(Path.Combine(this.InstancesFolder.FullName, instanceName));
            if (thisInstanceFolder.Exists)
            {
                FolderUtils.DeleteDirectory(thisInstanceFolder.FullName);
            }
            this.InstanceBank.InstancesInfoList.Remove(
                this.InstanceBank.InstancesInfoList.First(x => (x.Name.Equals(instanceName))));
            Logger.GetLogger().Debug(String.Format("Removed instance by instance name:{0}", instanceName));
            this.SaveInstancesToBankFile();
        }

        public String GetIconImage(String instanceName)
        {
            var folderPath = this.GetInstanceRootFolder(instanceName).FullName;
            var imagePath = Path.Combine(folderPath, "icon.png");
            return new FileInfo(imagePath).FullName;
        }

        public String GetBackgroundImage(String instanceName)
        {
            var folderPath = this.GetInstanceRootFolder(instanceName).FullName;
            var imagePath = Path.Combine(folderPath, "background.png");
            return new FileInfo(imagePath).FullName;
        }

        public void UpdateInstance(String instanceName)
        {
            //TODO:Update instance
        }

        public Process LaunchInstance(InternalNodeProgress progress, String instanceName, PlayerEntity player)
        {
            Logger.GetLogger().Info(String.Format("Start to launch {0} by player {1}...", instanceName, player.PlayerName));
            var instanceInfo = this.InstanceBank.InstancesInfoList.First(x => (x.Name.Equals(instanceName)));
            var instance =
                JsonConverter.Parse<InstanceEntity>(
                    File.ReadAllText(instanceInfo.FilePath));

            var instanceRootFolder = this.GetInstanceRootFolder(instance.InstanceName);
            var placer = new PlaceHolderReplacer();

            placer.AddToDictionary("{root}", instanceRootFolder.FullName.Replace(" ", "\" \""));
            placer.AddToDictionary("{username}", player.PlayerName ?? "Player");
            placer.AddToDictionary("{userId}", (!String.IsNullOrEmpty(player.PlayerId) ? player.PlayerId : Guid.NewGuid().ToString("N")));
            placer.AddToDictionary("{token}", String.IsNullOrEmpty(player.AccessToken) ? Guid.Empty.ToString("N") : player.AccessToken);
            //Buding environment
            //Try to extract entire file.
            if (instanceInfo.InstanceState == InstanceState.Initialize || instanceInfo.InstanceState == InstanceState.Update)
            {
                if (instance.FileSystem.EntirePackageFiles != null && instance.FileSystem.EntirePackageFiles.Count != 0)
                {
                    var singlePackageDownloadNodeProgress = 30D / instance.FileSystem.EntirePackageFiles.Count;
                    foreach (var entirePackageFile in instance.FileSystem.EntirePackageFiles)
                    {
                        entirePackageFile.LocalPath = placer.ReplaceArgument(entirePackageFile.LocalPath);
                        this.ReceiveEntirePackage(progress.CreateNewInternalSubProgress(singlePackageDownloadNodeProgress, String.Format("Receiving entire package {0}", entirePackageFile.Name)),
                            instance.InstanceName, entirePackageFile);
                    }
                }
            }

            progress.Percent = 30D;
            //Try to check all official files.
            if (instanceInfo.InstanceState == InstanceState.Initialize || instanceInfo.InstanceState == InstanceState.Update)
            {
                if (instance.FileSystem.OfficialFiles != null && instance.FileSystem.OfficialFiles.Count != 0)
                {
                    var singleOfficialDownloadNodeProgress = 30D / instance.FileSystem.OfficialFiles.Count;
                    foreach (var officialFileEntity in instance.FileSystem.OfficialFiles)
                    {
                        officialFileEntity.LocalPath = placer.ReplaceArgument(officialFileEntity.LocalPath);
                        this.ReceiveOfficialFile(
                            progress.CreateNewLeafSubProgress(singleOfficialDownloadNodeProgress,
                                String.Format("Downloading official file: {0}", officialFileEntity.Name)),
                            instance.InstanceName, officialFileEntity, this.UsingFileRepository);
                    }
                }
            }
            progress.Percent = 60D;


            //Try to check all custom files.
            if (instanceInfo.InstanceState == InstanceState.Initialize || instanceInfo.InstanceState == InstanceState.Update)
            {
                if (instance.FileSystem.CustomFiles != null && instance.FileSystem.CustomFiles.Count != 0)
                {
                    var singleCustomDownloadNodeProgress = 30D / instance.FileSystem.CustomFiles.Count;
                    foreach (var customFileEntity in instance.FileSystem.CustomFiles)
                    {
                        customFileEntity.LocalPath = placer.ReplaceArgument(customFileEntity.LocalPath);
                        this.ReceiveCustomFile(
                            progress.CreateNewLeafSubProgress(singleCustomDownloadNodeProgress,
                                String.Format("Downloading custom file: {0}", customFileEntity.Name)),
                            instance.InstanceName, customFileEntity);
                    }
                }
            }
            instanceInfo.InstanceState = InstanceState.Ok;
            this.SaveInstancesToBankFile();
            progress.Percent = 90D;
            //DONE:Build start argument.
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
            instanceProcess.EnableRaisingEvents = true;
            instanceProcess.Start();

            this.CurrentInstanceProcess = instanceProcess;
            Logger.GetLogger().Info(String.Format("Instance {0} launched!", instanceName));

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
            var repositoryFile = usingRepo.GetOfficialFile(officialFile.ProvideId);

            var downloadLink = repositoryFile.DownloadPath;
            var downloadTargetPositon = Path.Combine(this.GetInstanceRootFolder(instanceName).FullName, officialFile.LocalPath);

            ProgressSupportedDownloadUtils.DownloadFile(progress, downloadLink, downloadTargetPositon, officialFile.Md5);
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
