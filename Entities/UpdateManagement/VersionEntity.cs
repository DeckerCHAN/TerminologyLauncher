using System;

namespace TerminologyLauncher.Entities.UpdateManagement
{
    public class VersionEntity
    {
        public string CoreVersion { get; set; }
        public int BuildNumber { get; set; }
        public string DownloadLink { get; set; }
        public string Md5 { get; set; }
    }
}