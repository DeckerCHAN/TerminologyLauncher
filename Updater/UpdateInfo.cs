using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Entities.UpdateManagement;

namespace TerminologyLauncher.Updater
{
    public class UpdateInfo
    {
        public UpdateType UpdateType { get; set; }
        public VersionEntity LatestVersion { get; set; }
    }
}
