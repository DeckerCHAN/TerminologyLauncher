using System;

namespace TerminologyLauncher.Entities.InstanceManagement
{
    public class InstanceInfoEntity
    {
        public string Name { get; set; }
        public string UpdateUrl { get; set; }
        public string UpdateDate { get; set; }
        public InstanceState InstanceState { get; set; }
    }
}