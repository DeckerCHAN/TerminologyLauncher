using System;
using System.Collections.Generic;
using TerminologyLauncher.Entities.InstanceManagement.FileSystem;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public class InstanceFileSystemEntity
    {
        public List<EntirePackageFileEntity> EntirePackageFiles { get; set; }
        public List<OfficialFileEntity> OfficialFiles { get; set; }
        public List<CustomFileEntity> CustomFiles { get; set; }


    }
}
