using System;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public class InstanceEntity
    {
        public string Icon { get; set; }
        public string Background { get; set; }
        public string UpdatePath { get; set; }
        public string Version { get; set; }
        public int Generation { get; set; }
        public string InstanceName { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public InstanceFileSystemEntity FileSystem { get; set; }
        public InstanceStartupArgumentsEntity StartupArguments { get; set; }
    }
}