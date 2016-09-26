using System;

namespace TerminologyLauncher.Entities.InstanceManagement.FileSystem
{
    public abstract class RemoteFileEntity : FileBaseEntity
    {
        public string DownloadLink { get; set; }
        public string Md5 { get; set; }
    }
}
