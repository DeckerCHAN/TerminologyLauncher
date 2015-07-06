using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Entities.InstanceManagement.Local;
using TerminologyLauncher.Entities.InstanceManagement.Remote;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public class InstanceListEntity
    {
        public List<LocalInstanceEntity> InstanceEntities { get; set; }
    }
}
