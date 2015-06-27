using System;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public class CustomFileEntity : FileBaseEntity
    {
        public String Md5 { get; set; }
        public String DownloadPath { get; set; }
    }
}
