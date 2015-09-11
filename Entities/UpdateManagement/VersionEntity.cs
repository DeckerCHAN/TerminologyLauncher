using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.Entities.UpdateManagement
{
    public class VersionEntity
    {
        public String VersionNumber { get; set; }
        public String DownloadLink { get; set; }
        public String Md5 { get; set; }
    }
}
