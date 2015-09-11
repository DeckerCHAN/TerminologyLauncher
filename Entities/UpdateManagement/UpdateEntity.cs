using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.Entities.UpdateManagement
{
    public class UpdateEntity
    {
        public VersionEntity LatestVersion { get; set; }
        public List<VersionEntity> SupportingVersion { get; set; }
    }
}
