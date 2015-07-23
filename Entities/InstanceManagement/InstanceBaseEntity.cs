using System;
using TerminologyLauncher.Entities.InstanceManagement.Remote;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public abstract class InstanceBaseEntity
    {
        public String Icon { get; set; }
        public String Background { get; set; }
        public String Version { get; set; }
        public String InstanceName { get; set; }
        public String Description { get; set; }
        public String Author { get; set; }
        public InstanceFileSystemEntity FileSystem { get; set; }
        public String StartupArguments { get; set; }
    }
}
