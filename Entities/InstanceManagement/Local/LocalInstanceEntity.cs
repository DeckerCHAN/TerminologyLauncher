using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Entities.InstanceManagement.Remote;

namespace TerminologyLauncher.Entities.InstanceManagement.Local
{
    public class LocalInstanceEntity : InstanceBaseEntity
    {
        public String InstanceUpdateUrl { get; set; }
    }
}
