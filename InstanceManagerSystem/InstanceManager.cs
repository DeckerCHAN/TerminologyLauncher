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
        public InstanceManager(String configPath, FileRepository usingFileRepository, JreManager jreManager)
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
        public Int32 SupportGeneration { get { return 2; } }
        public DirectoryInfo InstancesFolder { get; set; }
        public InstanceBankEntity InstanceBank { get; set; }
        public JreManager JreManager { get; set; }
        public JavaRuntimeEntity JavaRuntime
        {
            get { return this.JreManager.JavaRuntime; }
        }

        public Process CurrentInstanceProcess { get; set; }
        public FileRepository UsingFileRepository { get; protected set; }

        public List<InstanceEntity> Instances
        {
            get
            {
                return this.InstanceBank.InstancesInfoList.Select(instanceInfo => JsonConverter.Parse<InstanceEntity>(File.ReadAllText(this.GetInstnaceFile(instanceInfo.Name)))).Where(instance => instance.Generation.Equals(this.SupportGeneration)).ToList();
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

            if (File.Exists(this.Config.GetConfigString("instanceBankFilePath")))
            {
                this.InstanceBank = JsonConverter.Parse<InstanceBankEntity>(File.ReadAllText(this.Config.GetConfigString("instanceBankFilePath")));
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
        public String AddInstance(String instanceUrl)
        {
            TerminologyLogger.GetLogger().Info(String.Format("Starting to add new instance through {0}.", instanceUrl));
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

        public String CheckAllInstanceCouldUpdate()
        {
            var availableUpdateInstanceNameList = new List<String>();
            foreach (var instanceInfoEntity in this.InstanceBank.InstancesInfoList)
            {
                TerminologyLogger.GetLogger().Info(String.Format("Check update {0}", instanceInfoEntity.Name));
                var instanceInfo = this.InstanceBank.InstancesInfoList.First(x => (x.Name.Equals(instanceInfoEntity.Name)));

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
                return String.Empty;
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
            return String.Format(TranslationManager.GetManager.Localize("ExistsInstanceAvailableToUpdate", "Detected {0} are available to update!", 1), result);
        }


        public String UpdateInstance(InternalNodeProgress progress, String instanceName)
        {
            TerminologyLogger.GetLogger().Info(String.Format("Start to update {0}", instanceName));
            var instanceInfo = this.InstanceBank.InstancesInfoList.First(x => (x.Name.Equals(instanceName)));

            var oldInstanceEntity =
                JsonConverter.Parse<InstanceEntity>(
                    File.ReadAllText(this.GetInstnaceFile(instanceInfo.Name)));
            this.CriticalInstanceFieldCheck(oldInstanceEntity);

            var newInstanceContent = DownloadUtils.GetWebContent(progress.CreateNewLeafSubProgress(String.Format("Receiving {0} instance file", instanceInfo.Name), 50D), instanceInfo.UpdateUrl);
            var newInstanceEntity = JsonConverter.Parse<InstanceEntity>(newInstanceContent);
            progress.Percent = 60D;
            //Check instance is available to update
            if (newInstanceEntity.Version.Equals(oldInstanceEntity.Version))
            {
                progress.Percent = 100D;
                TerminologyLogger.GetLogger().InfoFormat("Instacne {0} already at latest version {1}", oldInstanceEntity.InstanceName, oldInstanceEntity.Version);
                return (String.Format(TranslationManager.GetManager.Localize("InsatnceAtLatestVersion", "Instance now in latest version:{0}! Ignore update.", 1), newInstanceEntity.Version));
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
                        TerminologyLogger.GetLogger().InfoFormat("Successful updated instance file {0} from {1} to {2}!", newInstanceEntity.InstanceName, oldInstanceEntity.Version, newInstanceEntity.Version);
                        return String.Format(updateSuccessfulInfo,
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
                            if (!oldCustomFiles.Exists(x => x.Name.Equals(newCustomFileEntity.Name) && x.Md5.Equals(newCustomFileEntity.Md5)))
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
                        File.WriteAllText(this.GetInstnaceFile(instanceInfo.Name), JsonConverter.ConvertToJson(newInstanceEntity));
                        TerminologyLogger.GetLogger().InfoFormat("Successful updated entire instance {0} from {1} to {2}!", newInstanceEntity.InstanceName, oldInstanceEntity.Version, newInstanceEntity.Version);
                        return String.Format(updateSuccessfulInfo,
                            newInstanceEntity.InstanceName, oldInstanceEntity.Version, newInstanceEntity.Version);

                    }
                default:
                    {
                        throw new WrongStateException("Wrong instance state! Just instance which in OK or perinitialized state could update.");
                        break;
                    }

            }

        }

        public Process LaunchInstance(InternalNodeProgress progress, String instanceName, PlayerEntity player)
        {
            TerminologyLogger.GetLogger().Info(String.Format("Start to launch {0} by player {1}...", instanceName, player.PlayerName));
            var instanceInfo = this.InstanceBank.InstancesInfoList.First(x => (x.Name.Equals(instanceName)));
            var instance =
                JsonConverter.Parse<InstanceEntity>(
                    File.ReadAllText(this.GetInstnaceFile(instanceInfo.Name)));

            if (!(instanceInfo.InstanceState == InstanceState.Ok || instanceInfo.InstanceState == InstanceState.PerInitialized))
            {
                throw new WrongStateException("Wrong instance state! Just instance which in OK or PerInitialized state could launch.");
            }

            this.CriticalInstanceFieldCheck(instance);

            var instanceRootFolder = this.GetInstanceRootFolder(instance.InstanceName);

            #region Buding environment

            if (instanceInfo.InstanceState == InstanceState.PerInitialized)
            {
                #region entire file.
                if (instance.FileSystem.EntirePackageFiles != null && instance.FileSystem.EntirePackageFiles.Count != 0)
                {
                    var singlePackageDownloadNodeProgress = 30D / instance.FileSystem.EntirePackageFiles.Count;
                    foreach (var entirePackageFile in instance.FileSystem.EntirePackageFiles)
                    {
                        this.ReceiveEntirePackage(progress.CreateNewInternalSubProgress(String.Format("Receiving entire package {0}", entirePackageFile.Name), singlePackageDownloadNodeProgress),
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
                            progress.CreateNewLeafSubProgress(String.Format("Downloading official file: {0}", officialFileEntity.Name), singleOfficialDownloadNodeProgress),
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
                           progress.CreateNewLeafSubProgress(String.Format("Downloading custom file: {0}", customFileEntity.Name), singleCustomDownloadNodeProgress),
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
            var startArgument = new StringBuilder();// placer.ReplaceArgument(instance.StartupArguments);
            foreach (var jvmArgument in instance.StartupArguments.JvmArguments)
            {
                startArgument.Append(jvmArgument + " ");
            }

            startArgument.Append(this.Config.GetConfigString("extraJvmArguments") ?? String.Empty).Append(" ");

            startArgument.AppendFormat("-Xmx{0}M -Xms{1}M" + " ", Convert.ToInt64(this.Config.GetConfigString("maxMemorySizeMega")), instance.StartupArguments.MiniumMemoryMegaSize);

            var nativeFolder = new DirectoryInfo(Path.Combine(instanceRootFolder.FullName, instance.StartupArguments.Nativespath));
            if (nativeFolder.Exists)
            {
                startArgument.AppendFormat("-Djava.library.path=\"{0}\"" + " ", nativeFolder.FullName);

            }
            else
            {
                throw new DirectoryNotFoundException(String.Format("Native folder is not valid!"));
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
                    throw new FileNotFoundException(String.Format("Instance {0} is missing lib file {1}", instance.InstanceName, libraryPath));
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
                throw new FileNotFoundException(String.Format("Instance {0} is missing main jar file {1}", instance.InstanceName, mainJarFile.Name));
            }

            startArgument.Append(instance.StartupArguments.MainClass + " ");

            startArgument.AppendFormat("--username {0} ", player.PlayerName);
            startArgument.AppendFormat("--version {0} ", instance.StartupArguments.Version);
            startArgument.AppendFormat("--gameDir \"{0}\" ", instanceRootFolder.FullName);



            var assetsDir =
                new DirectoryInfo(Path.Combine(instanceRootFolder.FullName, instance.StartupArguments.AssetsDir));
            if (assetsDir.Exists)
            {
                startArgument.AppendFormat("--assetsDir \"{0}\" ", assetsDir.FullName);
            }
            else
            {
                throw new DirectoryNotFoundException("Assets folder not found!");
            }


            startArgument.AppendFormat("--assetIndex {0} ", instance.StartupArguments.AssetIndex);
            startArgument.AppendFormat("--uuid {0} ", player.PlayerId);
            startArgument.AppendFormat("--accessToken {0} ", player.AccessToken);
            startArgument.AppendFormat("--userProperties {{{0}}} ", instance.StartupArguments.UserProperties);
            startArgument.AppendFormat("--userType {0} ", instance.StartupArguments.UserType);

            foreach (var tweakClass in instance.StartupArguments.TweakClasses)
            {
                startArgument.AppendFormat("--tweakClass {0} ", tweakClass);
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
            var instanceProcess = new Process { StartInfo = instanceStartInfo, EnableRaisingEvents = true };
            instanceProcess.Start();
            progress.Percent = 100D;

            this.CurrentInstanceProcess = instanceProcess;
            TerminologyLogger.GetLogger().Info(String.Format("Instance {0} launched!", instanceName));

            return this.CurrentInstanceProcess;


        }
        #endregion
        #region Toolkits

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
            var repositoryFile = usingRepo.GetOfficialFile(progress, officialFile.ProvideId);


            var targetPositon = Path.Combine(this.GetInstanceRootFolder(instanceName).FullName, officialFile.LocalPath);
            repositoryFile.CopyTo(targetPositon, true);
            TerminologyLogger.GetLogger().Info(String.Format("Successfully put file:{0}.", targetPositon));

        }

        private void ReceiveCustomFile(LeafNodeProgress progress, String instanceName, CustomFileEntity customFile)
        {
            var downloadLink = customFile.DownloadLink;
            var downloadTargetPositon = Path.Combine(this.GetInstanceRootFolder(instanceName).FullName, customFile.LocalPath);
            TerminologyLogger.GetLogger().Info(String.Format("Downloading file:{0} from remote url:{1}.", downloadTargetPositon, downloadLink));
            DownloadUtils.DownloadFile(progress, downloadLink, downloadTargetPositon, customFile.Md5);
            TerminologyLogger.GetLogger().Info(String.Format("Successfully downloaded file:{0} from remote url:{1}.", downloadTargetPositon, downloadLink));
        }

        private void ReceiveEntirePackage(InternalNodeProgress progress, String instanceName, EntirePackageFileEntity entirePackageFile)
        {
            var downloadLink = entirePackageFile.DownloadLink;
            var downloadTargetPositon = Path.Combine(this.GetInstanceRootFolder(instanceName).FullName, entirePackageFile.LocalPath ?? String.Empty);
            TerminologyLogger.GetLogger().Info(String.Format("Downloading file:{0} from remote url:{1}.", downloadTargetPositon, downloadLink));
            DownloadUtils.DownloadZippedFile(progress, downloadLink, downloadTargetPositon, entirePackageFile.Md5);
            TerminologyLogger.GetLogger().Info(String.Format("Successfully downloaded file:{0} then extracted to {1}.", downloadLink, downloadTargetPositon));
        }

        private String GetIconImage(String instanceName)
        {
            var folderPath = this.GetInstanceRootFolder(instanceName).FullName;
            var imagePath = Path.Combine(folderPath, "icon.png");
            return new FileInfo(imagePath).FullName;
        }

        private String GetBackgroundImage(String instanceName)
        {
            var folderPath = this.GetInstanceRootFolder(instanceName).FullName;
            var imagePath = Path.Combine(folderPath, "background.png");
            return new FileInfo(imagePath).FullName;
        }

        private String GetInstnaceFile(String instanceName)
        {
            var folderPath = this.GetInstanceRootFolder(instanceName).FullName;
            var imagePath = Path.Combine(folderPath, "Instance.json");
            return new FileInfo(imagePath).FullName;
        }

        private void CriticalInstanceFieldCheck(InstanceEntity instance)
        {
            if (String.IsNullOrEmpty(instance.InstanceName))
            {
                throw new SolutionProvidedException("Instance is missing instance name! This is somehow critical error and you have to connect author to resolve this!");
            }
            if (!instance.Generation.Equals(this.SupportGeneration))
            {
                throw new SolutionProvidedException(String.Format("Current launcher not support {0} generation instance.", instance.Generation), " Using latest version for both launcher or instance my resolver this problem.");
            }
            if (String.IsNullOrEmpty(instance.UpdatePath))
            {
                throw new SolutionProvidedException(String.Format("Instance {0} is missing update url", instance.UpdatePath), "This problem should be report to instance author.");
            }

            if (String.IsNullOrEmpty(instance.Version))
            {
                throw new SolutionProvidedException(String.Format("Instance {0} is missing version number.", instance.Version), "This problem should be report to instance author.");
            }

            if (instance.StartupArguments.JvmArguments == null || instance.StartupArguments.JvmArguments.Count == 0)
            {
                throw new Exception(String.Format("Instance {0} is missing valid Jvm arguments!", instance.InstanceName));
            }

            if (String.IsNullOrEmpty(instance.StartupArguments.Nativespath))
            {
                throw new SolutionProvidedException(String.Format("Instance {0} is missing valid native path arguments!", instance.InstanceName), "This problem should be report to instance author.");
            }

            if (instance.StartupArguments.MiniumMemoryMegaSize > MachineUtils.GetTotalMemoryInMiB())
            {
                throw new SolutionProvidedException("Instance require memory over maxium machine memory!", "Extern machine memory may resolve problem.");
            }

            var javaDetail = this.JavaRuntime.JavaDetails;
            if (javaDetail.JavaType == JavaType.ClientX86 || javaDetail.JavaType == JavaType.ServerX86)
            {
                if (instance.StartupArguments.MiniumMemoryMegaSize >= 1600)
                {
                    throw new SolutionProvidedException("X86 Java may not allocate memory more then 1.6G!", "Using X64 Jre/Jdk may resolve this problem.");
                }
            }

            if (instance.StartupArguments.LibraryPaths == null || instance.StartupArguments.LibraryPaths.Count == 0)
            {
                throw new Exception(String.Format("Empty libraries path for instance {0} does not make sense!", instance.InstanceName));
            }

            if (String.IsNullOrEmpty(instance.StartupArguments.MainClass))
            {
                throw new Exception(String.Format("Empty main class for instance {0} is not allowed!", instance.InstanceName));
            }

            if (String.IsNullOrEmpty(instance.StartupArguments.MainJarPath))
            {
                throw new Exception(String.Format("Empty main jar path for instance {0} is not allowed!", instance.InstanceName));
            }

            if (String.IsNullOrEmpty(instance.StartupArguments.AssetsDir) ||
                String.IsNullOrEmpty(instance.StartupArguments.AssetIndex))
            {
                throw new Exception(String.Format("Empty assets arguments for instance {0} is not allowed!", instance.InstanceName));

            }

            var cmf = instance.FileSystem.CustomFiles ?? new List<CustomFileEntity>();
            var omf = instance.FileSystem.OfficialFiles ?? new List<OfficialFileEntity>();
            var etp = instance.FileSystem.EntirePackageFiles ?? new List<EntirePackageFileEntity>();
            if ((cmf.Count + omf.Count + etp.Count) == 0)
            {
                throw new Exception(String.Format("Instance {0} do not have any file! This should not happen and may cause divesting error!", instance.InstanceName));
            }
        }

        private void PerinitializeInstance(InstanceEntity instance)
        {
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
