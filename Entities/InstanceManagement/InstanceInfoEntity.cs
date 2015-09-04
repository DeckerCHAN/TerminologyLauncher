using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public class InstanceInfoEntity
    {
        public String Name { get; set; }
        public String FilePath { get; set; }
        public String UpdateUrl { get; set; }
        public String UpdateDate { get; set; }
        public InstanceState InstanceState { get; set; }
    }
}
