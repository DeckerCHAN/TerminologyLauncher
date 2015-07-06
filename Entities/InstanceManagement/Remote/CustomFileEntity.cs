using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.Entities.InstanceManagement.Remote
{
    public class CustomFileEntity:RemoteFileEntity
    {
        public String DownloadLink { get; set; }
    }
}
