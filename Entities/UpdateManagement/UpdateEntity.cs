using System.Collections.Generic;

namespace TerminologyLauncher.Entities.UpdateManagement
{
    public class UpdateEntity
    {
        public VersionEntity LatestVersion { get; set; }
        public List<VersionEntity> SupportingVersion { get; set; }
    }
}
