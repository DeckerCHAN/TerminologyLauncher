using System;
using Newtonsoft.Json.Serialization;

namespace TerminologyLauncher.Utils.SerializeUtils
{
    internal class ContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);
        }
    }
}