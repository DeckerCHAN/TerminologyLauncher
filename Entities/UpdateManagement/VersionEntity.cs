using System;

namespace TerminologyLauncher.Entities.UpdateManagement
{
    public class VersionEntity
    {
        public string CoreVersion { get; set; }
        public int BuildNumber { get; set; }
        public string DownloadLink { get; set; }
        public long Size { get; set; }
    }
}