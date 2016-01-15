using System;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public class InstanceInfoEntity
    {
        public String Name { get; set; }
        public String UpdateUrl { get; set; }
        public String UpdateDate { get; set; }
        public InstanceState InstanceState { get; set; }
    }
}
