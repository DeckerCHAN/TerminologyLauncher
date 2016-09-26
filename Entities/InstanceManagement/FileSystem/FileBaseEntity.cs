using System;

namespace TerminologyLauncher.Entities.InstanceManagement.FileSystem
{
    public abstract class FileBaseEntity
    {
        public string Name { get; set; }
        public string LocalPath { get; set; }
    }
}
