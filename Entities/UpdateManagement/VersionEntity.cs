using System;

namespace TerminologyLauncher.Entities.UpdateManagement
{
    public class VersionEntity
    {
        public String CoreVersion { get; set; }
        public Int32 BuildNumber { get; set; }
        public String DownloadLink { get; set; }
        public String Md5 { get; set; }
    }
}
