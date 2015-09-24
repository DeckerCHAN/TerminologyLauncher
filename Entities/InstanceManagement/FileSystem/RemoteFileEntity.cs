using System;

namespace TerminologyLauncher.Entities.InstanceManagement.FileSystem
{
    public abstract class RemoteFileEntity : FileBaseEntity
    {
        public String DownloadLink { get; set; }
        public String Md5 { get; set; }
    }
}
