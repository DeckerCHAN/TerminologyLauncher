using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public class LocalizedInstanceEntity : InstanceEntity
    {
        public string IconLocalPath { get; set; }
        public string BackgroundLocalPath { get; set; }
    }
}