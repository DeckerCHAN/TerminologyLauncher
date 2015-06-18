using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TerminologyLauncher.Entities.SerializeUtils
{
    internal class SerializeSettings : JsonSerializerSettings
    {
        public SerializeSettings()
        {
            this.ContractResolver = new ContractResolver();
            this.NullValueHandling = NullValueHandling.Ignore;
        }
    }
}
