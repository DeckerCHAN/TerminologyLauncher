using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public abstract class RemoteFileEntity : FileBaseEntity
    {
        public String DownloadLink { get; set; }
  
    }
}
