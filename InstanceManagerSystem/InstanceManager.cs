using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.InstanceManagement.FileSystem;
using TerminologyLauncher.Entities.System.Java;
using TerminologyLauncher.FileRepositorySystem;
using TerminologyLauncher.I18n;
using TerminologyLauncher.InstanceManagerSystem.Exceptions;
using TerminologyLauncher.JreManagerSystem;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.Exceptions;
using TerminologyLauncher.Utils.ProgressService;
using TerminologyLauncher.Utils.SerializeUtils;

namespace TerminologyLauncher.InstanceManagerSystem
{
    public class InstanceManager
    {
        public InstanceManager(string configPath, FileRepository usingFileRepository, JreManager jreManager)
        {
            this.Config = new Config(new FileInfo(configPath));
            this.UsingFileRepository = usingFileRepository;
            this.JreManager = jreManager;
            this.InstancesFolder = new DirectoryInfo(this.Config.GetConfigString("instancesFolderPath"));


            if (!this.InstancesFolder.Exists)
            {
                this.InstancesFolder.Create();
            }
            this.LoadInstancesFromBankFile();
        }

        public Config Config { get; set; }
        public int SupportGeneration => 2;
        public DirectoryInfo InstancesFolder { get; set; }
        public InstanceBankEntity InstanceBank { get; set; }
        public JreManager JreManager { get; set; }
        public JavaRuntimeEntity JavaRuntime => this.JreManager.JavaRuntime;

        public Process CurrentInstanceProcess { get; set; }
        public FileRepository UsingFileRepository { get; protected set; }

        public List<InstanceEntity> Instances
        {
            get
            {
                return
                    this.InstanceBank.InstancesInfoList.Select(
                            instanceInfo =>
                                JsonConverter.Parse<InstanceEntity>(
                                    File.ReadAllText(this.GetInstnaceFile(instanceInfo.Name))))
                        .Where(instance => instance.Generation.Equals(this.SupportGeneration))
                        .ToList();
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

        public InstanceEntity[] InstancesArray => this.Instances.ToArray();

        public void LoadInstancesFromBankFile()
        {
            if (File.Exists(this.Config.GetConfigString("instanceBankFilePath")))
            {
                this.InstanceBank =
                    JsonConverter.Parse<InstanceBankEntity>(
                        File.ReadAllText(this.Config.GetConfigString("instanceBankFilePath")));
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
            File.WriteAllText(this.Config.GetConfigString("instanceBankFilePath"), content);
        }

        #region Operation

        public string AddInstance(string instanceUrl)
        {
            TerminologyLogger.GetLogger().Info($"Starting to add new instance through {instanceUrl}.");
            this.LoadInstancesFromBankFile();
            //Download instance content
            var rowInstanceContent = DownloadUtils.GetWebContent(instanceUrl);

            var instance = JsonConverter.Parse<InstanceEntity>(rowInstanceContent);

            this.CriticalInstanceFieldCheck(instance);

            this.PerinitializeInstance(instance);

            var instanceInfo = new InstanceInfoEntity
            {
                Name = instance.InstanceName,
                InstanceState = InstanceState.PerInitialized,
                UpdateUrl = instance.UpdatePath,
                UpdateDate = DateTime.Now.ToString(CultureInfo.InvariantCulture)
            };


            File.WriteAllText(this.GetInstnaceFile(instanceInfo.Name), JsonConverter.ConvertToJson(instance));
            this.InstanceBank.InstancesInfoList.Add(instanceInfo);
            this.SaveInstancesBankToFile();
            return $"Added instance:{instance.InstanceName}";
        }

        public string RemoveInstance(string instanceName)
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

            return $"Removed instance by instance name:{instanceName}";
        }

        public string CheckAllInstanceCouldUpdate()
        {
            var availableUpdateInstanceNameList = new List<string>();
            foreach (var instanceInfoEntity in this.InstanceBank.InstancesInfoList)
            {
                TerminologyLogger.GetLogger().Info($"Check update {instanceInfoEntity.Name}");
                var instanceInfo =
                    this.InstanceBank.InstancesInfoList.First(x => (x.Name.Equals(instanceInfoEntity.Name)));

                var oldInstanceEntity =
                    JsonConverter.Parse<InstanceEntity>(
                        File.ReadAllText(this.GetInstnaceFile(instanceInfo.Name)));
                this.CriticalInstanceFieldCheck(oldInstanceEntity);

                var newInstanceContent = DownloadUtils.GetWebContent(instanceInfo.UpdateUrl);
                var newInstanceEntity = JsonConverter.Parse<InstanceEntity>(newInstanceContent);

                if (!newInstanceEntity.Version.Equals(oldInstanceEntity.Version))
                {
                    availableUpdateInstanceNameList.Add(instanceInfoEntity.Name);
                }
            }
            if (availableUpdateInstanceNameList.Count == 0)
            {
                TerminologyLogger.GetLogger().Info("All instances at latest version.");
                return string.Empty;
            }
            var result = new StringBuilder();
            for (var i = 0; i < availableUpdateInstanceNameList.Count; i++)
            {
                var name = availableUpdateInstanceNameList[i];
                result.Append(name);
                if (i != availableUpdateInstanceNameList.Count - 1)
                {
                    result.Append(", ");
                }
            }
            return
                string.Format(
                    TranslationManager.GetManager.Localize("ExistsInstanceAvailableToUpdate",
                        "Detected {0} are available to update!", 1), result);
        }


        public string UpdateInstance(InternalNodeProgress progress, string instanceName)
        {
            TerminologyLogger.GetLogger().Info($"Start to update {instanceName}");
            var instanceInfo = this.InstanceBank.InstancesInfoList.First(x => (x.Name.Equals(instanceName)));

            var oldInstanceEntity =
                JsonConverter.Parse<InstanceEntity>(
                    File.ReadAllText(this.GetInstnaceFile(instanceInfo.Name)));
            this.CriticalInstanceFieldCheck(oldInstanceEntity);

            var newInstanceContent = DownloadUtils.GetWebContent(progress.CreateNewLeafSubProgress(
                $"Receiving {instanceInfo.Name} instance file", 50D), instanceInfo.UpdateUrl);
            var newInstanceEntity = JsonConverter.Parse<InstanceEntity>(newInstanceContent);
            progress.Percent = 60D;
            //Check instance is available to update
            if (newInstanceEntity.Version.Equals(oldInstanceEntity.Version))
            {
                progress.Percent = 100D;
                TerminologyLogger.GetLogger()
                    .InfoFormat(
                        $"Instacne {oldInstanceEntity.InstanceName} already at latest version {oldInstanceEntity.Version}");
                return
                (string.Format(
                    TranslationManager.GetManager.Localize("InsatnceAtLatestVersion",
                        "Instance now in latest version:{0}! Ignore update.", 1), newInstanceEntity.Version));
            }
            var updateSuccessfulInfo = TranslationManager.GetManager.Localize("InstanceUpdateToVersion",
                "Successful update instance file {0} from version {1} to {2}!", 3);
            switch (instanceInfo.InstanceState)
            {
                case InstanceState.PerInitialized:
                {
                    this.RemoveInstance(instanceInfo.Name);
                    this.AddInstance(instanceInfo.UpdateUrl);
                    progress.Percent = 100D;
                    TerminologyLogger.GetLogger()
                        .InfoFormat(
                            $"Successful updated instance file {newInstanceEntity.InstanceName} from {oldInstanceEntity.Version} to {newInstanceEntity.Version}!");
                    return string.Format(updateSuccessfulInfo,
                        newInstanceEntity.InstanceName, oldInstanceEntity.Version, newInstanceEntity.Version);
                }

                case InstanceState.Ok:
                {
                    //TODO:I'll support instance name change at future version.
                    if (!newInstanceEntity.InstanceName.Equals(oldInstanceEntity.InstanceName))
                    {
                        throw new Exception("Old instance name not equal with new instance name.");
                    }

                    var instanceRootFolder = this.GetInstanceRootFolder(oldInstanceEntity.InstanceName);

                    #region Update files

                    #region Entire package

                    //Difference entire package will cause whole package target folder been delete


                    var newEntirePackages = newInstanceEntity.FileSystem.EntirePackageFiles ??
                                            new List<EntirePackageFileEntity>();
                    var oldEntirePackages = oldInstanceEntity.FileSystem.EntirePackageFiles ??
                                            new List<EntirePackageFileEntity>();
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
                            FolderUtils.DeleteDirectory(Path.Combine(instanceRootFolder.FullName,
                                oldEntirePackageFileEntity.LocalPath));
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

                    progress.Percent = 80D;

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

                    progress.Percent = 90D;

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
                        if (
                            !oldCustomFiles.Exists(
                                x => x.Name.Equals(newCustomFileEntity.Name) && x.Md5.Equals(newCustomFileEntity.Md5)))
                        {
                            this.ReceiveCustomFile(new LeafNodeProgress("Ignore"), newInstanceEntity.InstanceName,
                                newCustomFileEntity);
                        }
                    }

                    #endregion

                    progress.Percent = 100D;

                    #endregion

                    instanceInfo.UpdateDate = DateTime.Now.ToString("O");
                    this.SaveInstancesBankToFile();
                    File.WriteAllText(this.GetInstnaceFile(instanceInfo.Name),
                        JsonConverter.ConvertToJson(newInstanceEntity));
                    TerminologyLogger.GetLogger()
                        .InfoFormat(
                            $"Successful updated entire instance {newInstanceEntity.InstanceName} from {oldInstanceEntity.Version} to {newInstanceEntity.Version}!");
                    return string.Format(updateSuccessfulInfo,
                        newInstanceEntity.InstanceName, oldInstanceEntity.Version, newInstanceEntity.Version);
                }
                default:
                {
                    throw new WrongStateException(
                        "Wrong instance state! Just instance which in OK or perinitialized state could update.");
                    break;
                }
            }
        }

        public Process LaunchInstance(InternalNodeProgress progress, string instanceName, PlayerEntity player)
        {
            TerminologyLogger.GetLogger().Info($"Start to launch {instanceName} by player {player.PlayerName}...");
            var instanceInfo = this.InstanceBank.InstancesInfoList.First(x => (x.Name.Equals(instanceName)));
            var instance =
                JsonConverter.Parse<InstanceEntity>(
                    File.ReadAllText(this.GetInstnaceFile(instanceInfo.Name)));

            if (
                !(instanceInfo.InstanceState == InstanceState.Ok ||
                  instanceInfo.InstanceState == InstanceState.PerInitialized))
            {
                throw new WrongStateException(
                    "Wrong instance state! Just instance which in OK or PerInitialized state could launch.");
            }

            this.CriticalInstanceFieldCheck(instance);

            var instanceRootFolder = this.GetInstanceRootFolder(instance.InstanceName);

            #region Buding environment

            if (instanceInfo.InstanceState == InstanceState.PerInitialized)
            {
                #region entire file.

                if (instance.FileSystem.EntirePackageFiles != null && instance.FileSystem.EntirePackageFiles.Count != 0)
                {
                    var singlePackageDownloadNodeProgress = 30D/instance.FileSystem.EntirePackageFiles.Count;
                    foreach (var entirePackageFile in instance.FileSystem.EntirePackageFiles)
                    {
                        this.ReceiveEntirePackage(progress.CreateNewInternalSubProgress(
                                $"Receiving entire package {entirePackageFile.Name}", singlePackageDownloadNodeProgress),
                            instance.InstanceName, entirePackageFile);
                    }
                }

                #endregion

                progress.Percent = 30D;

                #region official files.

                if (instance.FileSystem.OfficialFiles != null && instance.FileSystem.OfficialFiles.Count != 0)
                {
                    var singleOfficialDownloadNodeProgress = 30D/instance.FileSystem.OfficialFiles.Count;
                    foreach (var officialFileEntity in instance.FileSystem.OfficialFiles)
                    {
                        this.ReceiveOfficialFile(
                            progress.CreateNewLeafSubProgress($"Downloading official file: {officialFileEntity.Name}",
                                singleOfficialDownloadNodeProgress),
                            instance.InstanceName, officialFileEntity, this.UsingFileRepository);
                    }
                }

                #endregion

                progress.Percent = 60D;

                #region custom files

                if (instance.FileSystem.CustomFiles != null && instance.FileSystem.CustomFiles.Count != 0)
                {
                    var singleCustomDownloadNodeProgress = 30D/instance.FileSystem.CustomFiles.Count;
                    foreach (var customFileEntity in instance.FileSystem.CustomFiles)
                    {
                        this.ReceiveCustomFile(
                            progress.CreateNewLeafSubProgress($"Downloading custom file: {customFileEntity.Name}",
                                singleCustomDownloadNodeProgress),
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
            var startArgument = new StringBuilder(); // placer.ReplaceArgument(instance.StartupArguments);
            foreach (var jvmArgument in instance.StartupArguments.JvmArguments)
            {
                startArgument.Append(jvmArgument + " ");
            }

            startArgument.Append(this.Config.GetConfigString("extraJvmArguments") ?? string.Empty).Append(" ");

            startArgument.Append(
                $"-Xmx{Convert.ToInt64(this.Config.GetConfigString("maxMemorySizeMega"))}M -Xms{instance.StartupArguments.MiniumMemoryMegaSize}M" +
                " ");

            var nativeFolder =
                new DirectoryInfo(Path.Combine(instanceRootFolder.FullName, instance.StartupArguments.Nativespath));
            if (nativeFolder.Exists)
            {
                startArgument.Append($"-Djava.library.path=\"{nativeFolder.FullName}\"" + " ");
            }
            else
            {
                throw new DirectoryNotFoundException(string.Format("Native folder is not valid!"));
            }

            startArgument.Append("-cp" + " ");

            foreach (var libraryPath in instance.StartupArguments.LibraryPaths)
            {
                var libFile = new FileInfo(Path.Combine(instanceRootFolder.FullName, libraryPath));
                if (libFile.Exists)
                {
                    startArgument.Append("\"" + libFile.FullName + "\"" + ";");
                }
                else
                {
                    throw new FileNotFoundException(
                        $"Instance {instance.InstanceName} is missing lib file {libraryPath}");
                }
            }

            var mainJarFile =
                new FileInfo(Path.Combine(instanceRootFolder.FullName, instance.StartupArguments.MainJarPath));

            if (mainJarFile.Exists)
            {
                startArgument.Append("\"" + mainJarFile.FullName + "\"" + " ");
            }
            else
            {
                throw new FileNotFoundException(
                    $"Instance {instance.InstanceName} is missing main jar file {mainJarFile.Name}");
            }

            startArgument.Append(instance.StartupArguments.MainClass + " ");

            startArgument.Append($"--username {player.PlayerName} ");
            startArgument.Append($"--version {instance.StartupArguments.Version} ");
            startArgument.Append($"--gameDir \"{instanceRootFolder.FullName}\" ");


            var assetsDir =
                new DirectoryInfo(Path.Combine(instanceRootFolder.FullName, instance.StartupArguments.AssetsDir));
            if (assetsDir.Exists)
            {
                startArgument.Append($"--assetsDir \"{assetsDir.FullName}\" ");
            }
            else
            {
                throw new DirectoryNotFoundException("Assets folder not found!");
            }


            startArgument.Append($"--assetIndex {instance.StartupArguments.AssetIndex} ");
            startArgument.Append($"--uuid {player.PlayerId} ");
            startArgument.Append($"--accessToken {player.AccessToken} ");
            startArgument.Append($"--userProperties {{{instance.StartupArguments.UserProperties}}} ");
            startArgument.Append($"--userType {instance.StartupArguments.UserType} ");

            foreach (var tweakClass in instance.StartupArguments.TweakClasses)
            {
                startArgument.Append($"--tweakClass {tweakClass} ");
            }


            //Launch minecraft
            var instanceStartInfo = new ProcessStartInfo
            {
                FileName = this.JavaRuntime.JavaWPath,
                Arguments = startArgument.ToString(),
                WorkingDirectory = instanceRootFolder.FullName,
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            var instanceProcess = new Process {StartInfo = instanceStartInfo, EnableRaisingEvents = true};
            instanceProcess.Start();
            progress.Percent = 100D;

            this.CurrentInstanceProcess = instanceProcess;
            TerminologyLogger.GetLogger().Info($"Instance {instanceName} launched!");

            return this.CurrentInstanceProcess;
        }

        #endregion

        #region Toolkits

        private DirectoryInfo GetInstanceRootFolder(string instanceName)
        {
            var folder = new DirectoryInfo(Path.Combine(this.InstancesFolder.FullName, instanceName));
            if (!folder.Exists)
            {
                throw new DirectoryNotFoundException($"Root folder for {instanceName} not exists.");
            }
            return folder;
        }

        private void ReceiveOfficialFile(LeafNodeProgress progress, string instanceName, OfficialFileEntity officialFile,
            FileRepository usingRepo)
        {
            //Try to find file at file repo 
            var repositoryFile = usingRepo.GetOfficialFile(progress, officialFile.ProvideId);


            var targetPositon = Path.Combine(this.GetInstanceRootFolder(instanceName).FullName, officialFile.LocalPath);
            repositoryFile.CopyTo(targetPositon, true);
            TerminologyLogger.GetLogger().Info($"Successfully put file:{targetPositon}.");
        }

        private void ReceiveCustomFile(LeafNodeProgress progress, string instanceName, CustomFileEntity customFile)
        {
            var downloadLink = customFile.DownloadLink;
            var downloadTargetPositon = Path.Combine(this.GetInstanceRootFolder(instanceName).FullName,
                customFile.LocalPath);
            TerminologyLogger.GetLogger().Info(
                $"Downloading file:{downloadTargetPositon} from remote url:{downloadLink}.");
            DownloadUtils.DownloadFile(progress, downloadLink, downloadTargetPositon, customFile.Md5);
            TerminologyLogger.GetLogger().Info(
                $"Successfully downloaded file:{downloadTargetPositon} from remote url:{downloadLink}.");
        }

        private void ReceiveEntirePackage(InternalNodeProgress progress, string instanceName,
            EntirePackageFileEntity entirePackageFile)
        {
            var downloadLink = entirePackageFile.DownloadLink;
            var downloadTargetPositon = Path.Combine(this.GetInstanceRootFolder(instanceName).FullName,
                entirePackageFile.LocalPath ?? string.Empty);
            TerminologyLogger.GetLogger().Info(
                $"Downloading file:{downloadTargetPositon} from remote url:{downloadLink}.");
            DownloadUtils.DownloadZippedFile(progress, downloadLink, downloadTargetPositon, entirePackageFile.Md5);
            TerminologyLogger.GetLogger().Info(
                $"Successfully downloaded file:{downloadLink} then extracted to {downloadTargetPositon}.");
        }

        private string GetIconImage(string instanceName)
        {
            var folderPath = this.GetInstanceRootFolder(instanceName).FullName;
            var imagePath = Path.Combine(folderPath, "icon.png");
            return new FileInfo(imagePath).FullName;
        }

        private string GetBackgroundImage(string instanceName)
        {
            var folderPath = this.GetInstanceRootFolder(instanceName).FullName;
            var imagePath = Path.Combine(folderPath, "background.png");
            return new FileInfo(imagePath).FullName;
        }

        private string GetInstnaceFile(string instanceName)
        {
            var folderPath = this.GetInstanceRootFolder(instanceName).FullName;
            var imagePath = Path.Combine(folderPath, "Instance.json");
            return new FileInfo(imagePath).FullName;
        }

        private void CriticalInstanceFieldCheck(InstanceEntity instance)
        {
            if (string.IsNullOrEmpty(instance.InstanceName))
            {
                throw new SolutionProvidedException(
                    "Instance is missing instance name! This is somehow critical error and you have to connect author to resolve this!");
            }
            if (!instance.Generation.Equals(this.SupportGeneration))
            {
                throw new SolutionProvidedException(
                    $"Current launcher not support {instance.Generation} generation instance.",
                    " Using latest version for both launcher or instance my resolver this problem.");
            }
            if (string.IsNullOrEmpty(instance.UpdatePath))
            {
                throw new SolutionProvidedException($"Instance {instance.UpdatePath} is missing update url",
                    "This problem should be report to instance author.");
            }

            if (string.IsNullOrEmpty(instance.Version))
            {
                throw new SolutionProvidedException($"Instance {instance.Version} is missing version number.",
                    "This problem should be report to instance author.");
            }

            if (instance.StartupArguments.JvmArguments == null || instance.StartupArguments.JvmArguments.Count == 0)
            {
                throw new Exception($"Instance {instance.InstanceName} is missing valid Jvm arguments!");
            }

            if (string.IsNullOrEmpty(instance.StartupArguments.Nativespath))
            {
                throw new SolutionProvidedException(
                    $"Instance {instance.InstanceName} is missing valid native path arguments!",
                    "This problem should be report to instance author.");
            }

            if (instance.StartupArguments.MiniumMemoryMegaSize > MachineUtils.GetTotalMemoryInMiB())
            {
                throw new SolutionProvidedException("Instance require memory over maxium machine memory!",
                    "Extern machine memory may resolve problem.");
            }

            var javaDetail = this.JavaRuntime.JavaDetails;
            if (javaDetail.JavaType == JavaType.ClientX86 || javaDetail.JavaType == JavaType.ServerX86)
            {
                if (instance.StartupArguments.MiniumMemoryMegaSize >= 1600)
                {
                    throw new SolutionProvidedException("X86 Java may not allocate memory more then 1.6G!",
                        "Using X64 Jre/Jdk may resolve this problem.");
                }
            }

            if (instance.StartupArguments.LibraryPaths == null || instance.StartupArguments.LibraryPaths.Count == 0)
            {
                throw new Exception($"Empty libraries path for instance {instance.InstanceName} does not make sense!");
            }

            if (string.IsNullOrEmpty(instance.StartupArguments.MainClass))
            {
                throw new Exception($"Empty main class for instance {instance.InstanceName} is not allowed!");
            }

            if (string.IsNullOrEmpty(instance.StartupArguments.MainJarPath))
            {
                throw new Exception($"Empty main jar path for instance {instance.InstanceName} is not allowed!");
            }

            if (string.IsNullOrEmpty(instance.StartupArguments.AssetsDir) ||
                string.IsNullOrEmpty(instance.StartupArguments.AssetIndex))
            {
                throw new Exception($"Empty assets arguments for instance {instance.InstanceName} is not allowed!");
            }

            var cmf = instance.FileSystem.CustomFiles ?? new List<CustomFileEntity>();
            var omf = instance.FileSystem.OfficialFiles ?? new List<OfficialFileEntity>();
            var etp = instance.FileSystem.EntirePackageFiles ?? new List<EntirePackageFileEntity>();
            if ((cmf.Count + omf.Count + etp.Count) == 0)
            {
                throw new Exception(
                    $"Instance {instance.InstanceName} do not have any file! This should not happen and may cause divesting error!");
            }
        }

        private void PerinitializeInstance(InstanceEntity instance)
        {
            //Check instance already exists
            if (this.Instances.Any(x => (x.InstanceName.Equals(instance.InstanceName))))
            {
                throw new InvalidOperationException($"Instance {instance.InstanceName} already exists!");
            }
            //Localize instance
            var thisInstanceFolder =
                new DirectoryInfo(Path.Combine(this.InstancesFolder.FullName, instance.InstanceName));
            if (thisInstanceFolder.Exists)
            {
                FolderUtils.DeleteDirectory(thisInstanceFolder.FullName);
            }
            thisInstanceFolder.Create();


            //Download icon
            var iconFile = new FileInfo(Path.Combine(thisInstanceFolder.FullName, "icon.png")).FullName;
            try
            {
                DownloadUtils.DownloadFile(instance.Icon, iconFile);
            }
            catch (WebException)
            {
                TerminologyLogger.GetLogger().Warn("Cannot download icon file.Using default instead.");
                ResourceUtils.CopyEmbedFileResource(
                    "TerminologyLauncher.InstanceManagerSystem.Resources.default_icon.png", new FileInfo(iconFile));
            }
            //Download background
            var bgFile = new FileInfo(Path.Combine(thisInstanceFolder.FullName, "background.png")).FullName;
            try
            {
                DownloadUtils.DownloadFile(instance.Background, bgFile);
            }
            catch (WebException)
            {
                TerminologyLogger.GetLogger().Warn("Cannot download background file.Using default instead.");
                ResourceUtils.CopyEmbedFileResource(
                    "TerminologyLauncher.InstanceManagerSystem.Resources.default_bg.png", new FileInfo(bgFile));
            }
            //TODO:encrypt instance file if request(next version).
        }

        #endregion
    }
}