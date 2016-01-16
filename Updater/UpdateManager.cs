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
        public UpdateManager(String configPath, String coreVersion, Int32 buildNumber)
        {
            this.Config = new Config(new FileInfo(configPath));
            this.Version = new VersionEntity() { BuildNumber = buildNumber, CoreVersion = coreVersion };
        }

        public Boolean IsNewVersionAvailable()
        {
            try
            {
                var latest = this.GetLatestVersion();
                if (latest.CoreVersion != this.Version.CoreVersion)
                {
                    return true;
                }
                return latest.BuildNumber != this.Version.BuildNumber;
            }
            catch (Exception ex)
            {
                Logging.Logger.GetLogger().ErrorFormat("Can not judge update available. Cause:{0}", ex);
                return false;
            }

        }

        public String GetUpdateInformationHumanReadable()
        {
            try
            {
                var latest = this.GetLatestVersion();
                if (latest.CoreVersion != this.Version.CoreVersion)
                {
                    return String.Format("New generation launcher {0}-{1} available.", latest.CoreVersion,
                        latest.BuildNumber);
                }
                else
                {
                    if (latest.BuildNumber > this.Version.BuildNumber)
                    {
                        return String.Format("Launcher has newer build version {0}", latest.BuildNumber);
                    }
                    else if (latest.BuildNumber == this.Version.BuildNumber)
                    {
                        return String.Format("You are using the latest version {0} build {1}", latest.CoreVersion, latest.BuildNumber);
                    }
                    else
                    {
                        return String.Format("The version you are using ({0}) is higher than official build({1}). You may using pre-release version or test version. PLEASE DO NOT DISTRIBUTE THIS VERSION UNLESS AUTHORISED.",
                            this.Version.BuildNumber, latest.BuildNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Logger.GetLogger().ErrorFormat("Can not detecte update available. Cause:{0}", ex);
                return String.Format("Cannot detecte update right now. Cause:{0}", ex.Message);
            }

        }

        private VersionEntity GetLatestVersion()
        {
            Logging.Logger.GetLogger().Info("Start to check lanucher update.");
            var update = JsonConverter.Parse<UpdateEntity>(DownloadUtils.GetWebContent(this.Config.GetConfigString("updateCheckingUrl")));
            if (update.LatestVersion == null || String.IsNullOrEmpty(update.LatestVersion.CoreVersion) || update.LatestVersion.BuildNumber.Equals(0) || String.IsNullOrEmpty(update.LatestVersion.DownloadLink) || String.IsNullOrEmpty(update.LatestVersion.Md5))
            {
                throw new UpdateServerErrorException("Cannot fetch the latest version!");
            }
            return update.LatestVersion;
        }

        public String FetchLatestVersionAndStartUpdate(InternalNodeProgress progress)
        {
            progress.Percent = 0D;

            if (!this.IsNewVersionAvailable())
            {
                return TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation.LanucherUpdateTranslation.NoAvailableUpdate;
            }

            var update = JsonConverter.Parse<UpdateEntity>(DownloadUtils.GetWebContent(this.Config.GetConfigString("updateCheckingUrl")));



            var updateTempFolder = Path.Combine(FolderUtils.SystemTempFolder.FullName,
                String.Format("TerminologyLauncher-{0}-{1}", update.LatestVersion.CoreVersion, update.LatestVersion.BuildNumber));
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
                    String.Format("{0} {1} {2}", updateBinaryFolder,
                        Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Assembly.GetEntryAssembly().Location),
                CreateNoWindow = false,
                UseShellExecute = true
            };
            var updateProcess = new Process { StartInfo = updateProcessInfo };
            updateProcess.Start();

            progress.Percent = 100D;
            return String.Format(TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation.LanucherUpdateTranslation.FetchedNewUpdateToVersion, String.Format("{0}-{1}", this.Version.CoreVersion, this.Version.BuildNumber),
   String.Format("{0}-{1}", this.Version.CoreVersion, this.Version.BuildNumber));
        }

    }
}
