using System.Collections.Generic;

namespace TerminologyLauncher.Entities.InstanceManagement.Remote
{
    public class InstanceFileSystemEntity
    {
        public EntirePackageFileEntity EntirePackageFile { get; set; }
        public List<OfficialFileEntity> OfficialFiles { get; set; }
        public List<CustomFileEntity> CustomFiles { get; set; }  


    }
}
