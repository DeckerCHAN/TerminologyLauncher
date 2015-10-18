using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.SerializeUtils;
using TerminologyLauncher.Entities.UpdateManagement;
using TerminologyLauncher.I18n;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Updater
{
    public class UpdateManager
    {
        public String Version { get; private set; }
        public Config Config { get; set; }
        public UpdateManager(String configPath, String versionNumber)
        {
            this.Config = new Config(new FileInfo(configPath));
            this.Version = versionNumber;
        }

        public Boolean CheckUpdateAvailable()
        {
            Logging.Logger.GetLogger().Info("Start to check lanucher version.");
            var update = JsonConverter.Parse<UpdateEntity>(DownloadUtils.GetWebContent(this.Config.GetConfigString("updateCheckingUrl")));
            if (update.LatestVersion == null)
            {
                throw new PlatformNotSupportedException("Can not fetch the latest version! May caused by wrong update url or un-supported version!");
            }
            var result = !String.IsNullOrEmpty(update.LatestVersion.VersionNumber) &&
                   update.LatestVersion.VersionNumber != this.Version;
            if (result)
            {
                Logging.Logger.GetLogger().InfoFormat("New version {0} available!", update.LatestVersion);
            }
            else
            {
                Logging.Logger.GetLogger().Info("No available version.");
            }
            return result;
        }

        public String FetchLatestVersionAndStartUpdate(InternalNodeProgress progress)
        {
            progress.Percent = 0D;

            if (!this.CheckUpdateAvailable())
            {
                return TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation.LanucherUpdateTranslation.NoAvailableUpdate;
            }

            var update = JsonConverter.Parse<UpdateEntity>(DownloadUtils.GetWebContent(this.Config.GetConfigString("updateCheckingUrl")));



            var updateTempFolder = Path.Combine(FolderUtils.SystemTempFolder.FullName,
                String.Format("TerminologyLauncher-{0}", update.LatestVersion.VersionNumber));
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
                throw new FileNotFoundException("Can not find updater.exe. You may have to re-install to resolve!");
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
            return String.Format(TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation.LanucherUpdateTranslation.FetchedNewUpdateToVersion, this.Version,
    update.LatestVersion.VersionNumber);
        }

    }
}
