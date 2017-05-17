using System.Collections.Generic;
using TerminologyLauncher.Entities.InstanceManagement.FileSystem;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public class InstanceFileSystemEntity
    {
        public ICollection<EntirePackageFileEntity> EntirePackageFiles { get; set; }
        public ICollection<OfficialFileEntity> OfficialFiles { get; set; }
        public ICollection<CustomFileEntity> CustomFiles { get; set; }
    }
}