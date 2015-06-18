using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace TerminologyLauncher.Entities.SerializeUtils
{
    internal class ContractResolver : DefaultContractResolver
    {

        protected override string ResolvePropertyName(string propertyName)
        {
            return Char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);
        }

    }
}
