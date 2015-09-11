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
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Updater
{
    public class UpdateManager
    {
        public String Version { get; private set; }
        public Config Config { get; set; }
        public UpdateManager(String versionNumber)
        {
            this.Version = versionNumber;
        }

        public Boolean CheckUpdateAvailable()
        {
            var update = JsonConverter.Parse<UpdateEntity>(DownloadUtils.GetFileContent(this.Config.GetConfig("updateCheckingUrl")));
            if (update.LatestVersion == null)
            {
                throw new PlatformNotSupportedException("Can not fetch the latest version! May caused by wrong update url or un-supported version!");
            }
            return !String.IsNullOrEmpty(update.LatestVersion.VersionNumber) &&
                   update.LatestVersion.VersionNumber != this.Version;
        }

        public String FetchLatestVersionAndStartUpdate(InternalNodeProgress progress)
        {
            progress.Percent = 0D;

            if (!this.CheckUpdateAvailable())
            {
                return "No newer update available.";
            }

            var update = JsonConverter.Parse<UpdateEntity>(DownloadUtils.GetFileContent(this.Config.GetConfig("updateCheckingUrl")));



            var updateTempFolder = Path.Combine(DownloadUtils.SystemTempFolder.FullName,
                String.Format("TerminologyLauncher-{0}", update.LatestVersion.VersionNumber));
            var updateBinaryFolder = Path.Combine(updateTempFolder, "Binary");
            var updaterExecutorFile = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Updater.exe");
            var updaterRealExecutorFile = Path.Combine(updateTempFolder, "Updater.exe");

            progress.Percent = 10D;
            ProgressSupportedDownloadUtils.DownloadZippedFile(
                progress.CreateNewInternalSubProgress(80D, "Fetching update pack"), update.LatestVersion.DownloadLink,
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
                    String.Format("{0} {1}", updateBinaryFolder,
                        Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)),
                CreateNoWindow = false
            };
            var updateProcess = new Process { StartInfo = updateProcessInfo };
            updateProcess.Start();

            progress.Percent = 100D;
            return String.Format("Updating from {0} to {1}! Please close current launcher to continue.", this.Version,
    update.LatestVersion.VersionNumber);
        }

    }
}
