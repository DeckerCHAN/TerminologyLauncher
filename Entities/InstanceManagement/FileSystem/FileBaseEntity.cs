using System;

namespace TerminologyLauncher.Entities.InstanceManagement.FileSystem
{
    public abstract class FileBaseEntity
    {
        public String Name { get; set; }
        public String Md5 { get; set; }
        public String LocalPath { get; set; }
    }
}
