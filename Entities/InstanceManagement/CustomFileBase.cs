using System;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public class CustomFileBase : FileBase
    {
        public String Md5 { get; set; }
        public String DownloadPath { get; set; }
    }
}
