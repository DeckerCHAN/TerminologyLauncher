using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.UpdateManagement;
using TerminologyLauncher.I18n;
using TerminologyLauncher.Updater.Exceptions.Update;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.ProgressService;
using TerminologyLauncher.Utils.SerializeUtils;

namespace TerminologyLauncher.Updater
{
    public class UpdateManager
    {
        public VersionEntity Version { get; set; }
        public Config Config { get; set; }

        public UpdateManager(string configPath, string coreVersion, int buildNumber)
        {
            this.Config = new Config(new FileInfo(configPath));
            this.Version = new VersionEntity() {BuildNumber = buildNumber, CoreVersion = coreVersion};
        }

        public UpdateInfo GetupdateInfo()
        {
            var updateInfo = new UpdateInfo()
            {
                UpdateType = UpdateType.Equal,
                LatestVersion = this.GetLatestVersion()
            };
            try
            {
                if (updateInfo.LatestVersion.CoreVersion != this.Version.CoreVersion)
                {
                    for (var i = 0; i < updateInfo.LatestVersion.CoreVersion.ToCharArray().Length; i++)
                    {
                        var newChar = updateInfo.LatestVersion.CoreVersion.ToCharArray()[i];
                        var oldChar = this.Version.CoreVersion[i];
                        if (newChar > oldChar)
                        {
                            updateInfo.UpdateType = UpdateType.Higher;
                            break;
                        }
                        else if (newChar < oldChar)
                        {
                            updateInfo.UpdateType = UpdateType.Lower;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    if (updateInfo.LatestVersion.BuildNumber > this.Version.BuildNumber)
                    {
                        updateInfo.UpdateType = UpdateType.Higher;
                    }
                    else if (updateInfo.LatestVersion.BuildNumber == this.Version.BuildNumber)
                    {
                        updateInfo.UpdateType = UpdateType.Equal;
                    }
                    else
                    {
                        updateInfo.UpdateType = UpdateType.Lower;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.TerminologyLogger.GetLogger().ErrorFormat($"Can not judge update available. Cause:{ex}");
                updateInfo.UpdateType = UpdateType.Equal;
            }
            return updateInfo;
        }


        private VersionEntity GetLatestVersion()
        {
            Logging.TerminologyLogger.GetLogger().Info("Start to check lanucher update.");
            var update =
                JsonConverter.Parse<UpdateEntity>(
                    DownloadUtils.GetWebContent(this.Config.GetConfigString("updateCheckingUrl")));
            if (update.LatestVersion == null || string.IsNullOrEmpty(update.LatestVersion.CoreVersion) ||
                update.LatestVersion.BuildNumber.Equals(0) || string.IsNullOrEmpty(update.LatestVersion.DownloadLink) ||
                string.IsNullOrEmpty(update.LatestVersion.Md5))
            {
                throw new UpdateServerErrorException("Cannot fetch the latest version!");
            }
            return update.LatestVersion;
        }

        public string FetchLatestVersionAndStartUpdate(InternalNodeProgress progress)
        {
            progress.Percent = 0D;

            if (!this.GetupdateInfo().UpdateType.Equals(UpdateType.Higher))
            {
                return TranslationManager.GetManager.Localize("NoUpdateAvailable", "No update available.");
            }

            var update =
                JsonConverter.Parse<UpdateEntity>(
                    DownloadUtils.GetWebContent(this.Config.GetConfigString("updateCheckingUrl")));


            var updateTempFolder = Path.Combine(FolderUtils.SystemTempFolder.FullName,
                $"TerminologyLauncher-{update.LatestVersion.CoreVersion}-{update.LatestVersion.BuildNumber}");
            var updateBinaryFolder = Path.Combine(updateTempFolder, "Binary");
            var updaterExecutorFile = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Updater.exe");
            var updaterRealExecutorFile = Path.Combine(updateTempFolder, "Updater.exe");

            if (Directory.Exists(updateTempFolder))
            {
                FolderUtils.DeleteDirectory(updateTempFolder);
            }

            progress.Percent = 10D;
            DownloadUtils.DownloadZippedFile(
                progress.CreateNewInternalSubProgress("Fetching update pack", 80D), update.LatestVersion.DownloadLink,
                updateBinaryFolder,
                update.LatestVersion.Md5);
            if (!File.Exists(updaterExecutorFile))
            {
                throw new FileNotFoundException("Cannot find updater.exe. You may have to re-install to resolve!");
            }
            File.Copy("Updater.exe", updaterRealExecutorFile, true);
            var updateProcessInfo = new ProcessStartInfo(updaterRealExecutorFile)
            {
                Arguments =
                    $"{updateBinaryFolder} {Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)} {Assembly.GetEntryAssembly().Location}",
                CreateNoWindow = false,
                UseShellExecute = true
            };
            var updateProcess = new Process {StartInfo = updateProcessInfo};
            updateProcess.Start();

            progress.Percent = 100D;
            return
                string.Format(
                    TranslationManager.GetManager.Localize("FetchedNewUpdateToVersion",
                        "Updating from {0} to {1}! Close launcher to continue.", 2),
                    $"{this.Version.CoreVersion}-{this.Version.BuildNumber}",
                    $"{this.Version.CoreVersion}-{this.Version.BuildNumber}");
        }
    }
}