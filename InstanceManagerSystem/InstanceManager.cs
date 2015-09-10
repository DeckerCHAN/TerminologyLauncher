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
using TerminologyLauncher.InstanceManagerSystem.Exceptions;
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

        public void SaveInstancesBankToFile()
        {
            var content = JsonConverter.ConvertToJson(this.InstanceBank);
            File.WriteAllText(this.Config.GetConfig("instanceBankFilePath"), content);
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

        public String AddInstance(String instanceUrl)
        {
            Logger.GetLogger().Info(String.Format("Starting to add new instance through {0}.", instanceUrl));
            this.LoadInstancesFromBankFile();
            //Download instance content
            var rowInstanceContent = DownloadUtils.GetFileContent(instanceUrl);

            var instance = JsonConverter.Parse<InstanceEntity>(rowInstanceContent);

            if (!instance.Generation.Equals(this.Config.GetConfig("supportInstanceGeneration")))
            {
                throw new NotSupportedException(String.Format("Current launcher not support {0} generation instance. Using latest version for both launcher or instance my resolver this problem.", instance.Generation));
            }

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
            this.SaveInstancesBankToFile();
            return String.Format("Added instance:{0}", instance.InstanceName);

        }

        public String RemoveInstance(String instanceName)
        {
            this.LoadInstancesFromBankFile();

            var thisInstanceFolder = new DirectoryInfo(Path.Combine(this.InstancesFolder.FullName, instanceName));
            if (thisInstanceFolder.Exists)
            {
                FolderUtils.DeleteDirectory(thisInstanceFolder.FullName);
            }
            this.InstanceBank.InstancesInfoList.Remove(
                this.InstanceBank.InstancesInfoList.First(x => (x.Name.Equals(instanceName))));
            this.SaveInstancesBankToFile();

            return String.Format("Removed instance by instance name:{0}", instanceName);

        }

        public String UpdateInstance(InternalNodeProgress progress, String instanceName)
        {
            Logger.GetLogger().Info(String.Format("Start to update {0}", instanceName));
            var instanceInfo = this.InstanceBank.InstancesInfoList.First(x => (x.Name.Equals(instanceName)));
            if (instanceInfo.InstanceState != InstanceState.Ok)
            {
                throw new WrongStateException("Wrong instance state! Just instance which in OK state could update.");
            }

            var oldInstanceEntity =
                JsonConverter.Parse<InstanceEntity>(
                    File.ReadAllText(instanceInfo.FilePath));

            if (String.IsNullOrEmpty(instanceInfo.UpdateUrl))
            {
                throw new Exception("Empty update url is not allowed.");
            }

            var newInstanceContent = DownloadUtils.GetFileContent(instanceInfo.UpdateUrl);
            var newInstanceEntity = JsonConverter.Parse<InstanceEntity>(newInstanceContent);

            //Check instance is allowed to update
            if (newInstanceEntity.Version == oldInstanceEntity.Version)
            {
                throw new NoAvailableUpdateException(String.Format("Instance now in latest version:{0}! Ignore update.", newInstanceEntity.Version));
            }

            //TODO:I'll support instance name change at feature version.
            if (!newInstanceEntity.InstanceName.Equals(oldInstanceEntity.InstanceName))
            {
                throw new Exception("Old instance name not equal with new instance name.");
            }

            var instanceRootFolder = this.GetInstanceRootFolder(oldInstanceEntity.InstanceName);

            #region Update files

            #region Entire package
            //Difference entire package will cause whole package target folder been delete

            instanceInfo.InstanceState = InstanceState.Update;

            var newEntirePackages = newInstanceEntity.FileSystem.EntirePackageFiles ?? new List<EntirePackageFileEntity>();
            var oldEntirePackages = oldInstanceEntity.FileSystem.EntirePackageFiles ?? new List<EntirePackageFileEntity>();
            //Delete old entire package
            foreach (var oldEntirePackageFileEntity in oldEntirePackages)
            {
                if (!newEntirePackages.Exists
                        (
                            x => x.Name.Equals(oldEntirePackageFileEntity.Name) &&
                                x.Md5.Equals(oldEntirePackageFileEntity.Md5)
                        )
                    )
                {
                    FolderUtils.DeleteDirectory(Path.Combine(instanceRootFolder.FullName, oldEntirePackageFileEntity.LocalPath));
                }
            }
            //Download new entire package
            foreach (var newEntirePackageFileEntity in newEntirePackages)
            {
                if (!oldEntirePackages.Exists
                    (
                        x =>
                            x.Name.Equals(newEntirePackageFileEntity.Name) &&
                            x.Md5.Equals(newEntirePackageFileEntity.Md5)
                    )
                    )
                {
                    //TODO:Support progress
                    this.ReceiveEntirePackage(new InternalNodeProgress("Ignore"), newInstanceEntity.InstanceName,
                        newEntirePackageFileEntity);
                }
            }
            #endregion
            progress.Percent = 30D;
            #region Official files

            var newOfficialFiles = newInstanceEntity.FileSystem.OfficialFiles ?? new List<OfficialFileEntity>();
            var oldOfficialFiles = newInstanceEntity.FileSystem.OfficialFiles ?? new List<OfficialFileEntity>();
            //Delete old official files
            foreach (var oldOfficialFileEntity in oldOfficialFiles)
            {
                if (!newOfficialFiles.Exists(x => x.ProvideId.Equals(oldOfficialFileEntity.ProvideId)))
                {
                    File.Delete(Path.Combine(instanceRootFolder.FullName, oldOfficialFileEntity.LocalPath));
                }
            }
            //Receive new official files
            foreach (var newOfficialFileEntity in newOfficialFiles)
            {
                if (!oldOfficialFiles.Exists(x => x.ProvideId.Equals(newOfficialFileEntity.ProvideId)))
                {
                    this.ReceiveOfficialFile(new LeafNodeProgress("Ignore"), newInstanceEntity.InstanceName,
                        newOfficialFileEntity, this.UsingFileRepository);
                }
            }
            #endregion

            progress.Percent = 60D;
            #region Custom files

            var newCustomFiles = newInstanceEntity.FileSystem.CustomFiles ?? new List<CustomFileEntity>();
            var oldCustomFiles = oldInstanceEntity.FileSystem.CustomFiles ?? new List<CustomFileEntity>();
            foreach (var oldCustomFileEntity in oldCustomFiles)
            {
                if (
                    !newCustomFiles.Exists(
                        x => x.Name.Equals(oldCustomFileEntity.Name) && x.Md5.Equals(oldCustomFileEntity.Md5)))
                {
                    File.Delete(Path.Combine(instanceRootFolder.FullName, oldCustomFileEntity.LocalPath));
                }
            }
            foreach (var newCustomFileEntity in newCustomFiles)
            {
                if (!oldCustomFiles.Exists(x => x.Name.Equals(newCustomFileEntity.Name) && x.Md5.Equals(newCustomFileEntity.Md5)))
                {
                    this.ReceiveCustomFile(new LeafNodeProgress("Ignore"), newInstanceEntity.InstanceName,
                        newCustomFileEntity);
                }
            }
            #endregion
            progress.Percent = 90D;
            #endregion

            instanceInfo.InstanceState = InstanceState.Ok;
            instanceInfo.UpdateDate = DateTime.Now.ToString("O");
            this.SaveInstancesBankToFile();
            File.WriteAllText(instanceInfo.FilePath, JsonConverter.ConvertToJson(newInstanceEntity));
            progress.Percent = 100D;
            return String.Format("Successful update instance {0} from version {1} to {2}!",
                newInstanceEntity.InstanceName, oldInstanceEntity.Version, newInstanceEntity.Version);
        }

        public Process LaunchInstance(InternalNodeProgress progress, String instanceName, PlayerEntity player)
        {
            Logger.GetLogger().Info(String.Format("Start to launch {0} by player {1}...", instanceName, player.PlayerName));
            var instanceInfo = this.InstanceBank.InstancesInfoList.First(x => (x.Name.Equals(instanceName)));
            var instance =
                JsonConverter.Parse<InstanceEntity>(
                    File.ReadAllText(instanceInfo.FilePath));

            if (!(instanceInfo.InstanceState == InstanceState.Ok || instanceInfo.InstanceState == InstanceState.Initialize))
            {
                throw new WrongStateException("Wrong instance state! Just instance which in OK or Initialize state could launch.");
            }

            var instanceRootFolder = this.GetInstanceRootFolder(instance.InstanceName);

            var placer = new PlaceHolderReplacer();
            placer.AddToDictionary("{root}", instanceRootFolder.FullName.Replace(" ", "\" \""));
            placer.AddToDictionary("{username}", player.PlayerName ?? "Player");
            placer.AddToDictionary("{userId}", player.PlayerId);
            placer.AddToDictionary("{token}", player.AccessToken);

            #region Buding environment

            if (instanceInfo.InstanceState == InstanceState.Initialize)
            {
                #region entire file.
                if (instance.FileSystem.EntirePackageFiles != null && instance.FileSystem.EntirePackageFiles.Count != 0)
                {
                    var singlePackageDownloadNodeProgress = 30D / instance.FileSystem.EntirePackageFiles.Count;
                    foreach (var entirePackageFile in instance.FileSystem.EntirePackageFiles)
                    {
                        this.ReceiveEntirePackage(progress.CreateNewInternalSubProgress(singlePackageDownloadNodeProgress, String.Format("Receiving entire package {0}", entirePackageFile.Name)),
                        instance.InstanceName, entirePackageFile);
                    }
                }
                #endregion
                progress.Percent = 30D;
                #region official files.
                if (instance.FileSystem.OfficialFiles != null && instance.FileSystem.OfficialFiles.Count != 0)
                {
                    var singleOfficialDownloadNodeProgress = 30D / instance.FileSystem.OfficialFiles.Count;
                    foreach (var officialFileEntity in instance.FileSystem.OfficialFiles)
                    {
                        this.ReceiveOfficialFile(
                            progress.CreateNewLeafSubProgress(singleOfficialDownloadNodeProgress,
                                String.Format("Downloading official file: {0}", officialFileEntity.Name)),
                            instance.InstanceName, officialFileEntity, this.UsingFileRepository);
                    }
                }
                #endregion
                progress.Percent = 60D;
                #region custom files

                if (instance.FileSystem.CustomFiles != null && instance.FileSystem.CustomFiles.Count != 0)
                {
                    var singleCustomDownloadNodeProgress = 30D / instance.FileSystem.CustomFiles.Count;
                    foreach (var customFileEntity in instance.FileSystem.CustomFiles)
                    {
                        this.ReceiveCustomFile(
                           progress.CreateNewLeafSubProgress(singleCustomDownloadNodeProgress,
                               String.Format("Downloading custom file: {0}", customFileEntity.Name)),
                           instance.InstanceName, customFileEntity);
                    }
                }
                #endregion
            }

            #endregion
            progress.Percent = 90D;

            instanceInfo.InstanceState = InstanceState.Ok;
            instanceInfo.UpdateDate = DateTime.Now.ToString("O");
            this.SaveInstancesBankToFile();

            //DONE:Build start argument.
            var startArgument = placer.ReplaceArgument(instance.StartupArguments);


            //Launch minecraft
            var instanceStartInfo = new ProcessStartInfo();
            var instanceProcess = new Process();
            instanceStartInfo.FileName = this.Config.GetConfig("javaPath");
            instanceStartInfo.Arguments = startArgument;
            instanceStartInfo.WorkingDirectory = instanceRootFolder.FullName;
            instanceStartInfo.WindowStyle = ProcessWindowStyle.Normal;
            instanceStartInfo.UseShellExecute = false;
            instanceStartInfo.RedirectStandardOutput = true;
            instanceStartInfo.UseShellExecute = false;
            instanceProcess.StartInfo = instanceStartInfo;
            instanceProcess.EnableRaisingEvents = true;
            instanceProcess.Start();
            progress.Percent = 100D;

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
            Logger.GetLogger().Info(String.Format("Downloading file:{0} from remote url:{1}.", downloadTargetPositon, downloadLink));
            ProgressSupportedDownloadUtils.DownloadFile(progress, downloadLink, downloadTargetPositon, officialFile.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} from remote url:{1}.", downloadTargetPositon, downloadLink));

        }

        private void ReceiveCustomFile(LeafNodeProgress progress, String instanceName, CustomFileEntity customFile)
        {
            var downloadLink = customFile.DownloadLink;
            var downloadTargetPositon = Path.Combine(this.GetInstanceRootFolder(instanceName).FullName, customFile.LocalPath);
            Logger.GetLogger().Info(String.Format("Downloading file:{0} from remote url:{1}.", downloadTargetPositon, downloadLink));
            ProgressSupportedDownloadUtils.DownloadFile(progress, downloadLink, downloadTargetPositon, customFile.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} from remote url:{1}.", downloadTargetPositon, downloadLink));
        }

        private void ReceiveEntirePackage(InternalNodeProgress progress, String instanceName, EntirePackageFileEntity entirePackageFile)
        {
            var downloadLink = entirePackageFile.DownloadLink;
            var downloadTargetPositon = Path.Combine(this.GetInstanceRootFolder(instanceName).FullName, entirePackageFile.LocalPath ?? String.Empty);
            Logger.GetLogger().Info(String.Format("Downloading file:{0} from remote url:{1}.", downloadTargetPositon, downloadLink));
            ProgressSupportedDownloadUtils.DownloadZippedFile(progress, downloadLink, downloadTargetPositon, entirePackageFile.Md5);
            Logger.GetLogger().Info(String.Format("Successfully downloaded file:{0} then extracted to {1}.", downloadLink, downloadTargetPositon));
        }
    }
}
