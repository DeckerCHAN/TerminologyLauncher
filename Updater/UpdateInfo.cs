using TerminologyLauncher.Entities.UpdateManagement;

namespace TerminologyLauncher.Updater
{
    public class UpdateInfo
    {
        public UpdateType UpdateType { get; set; }
        public VersionEntity LatestVersion { get; set; }
    }
}