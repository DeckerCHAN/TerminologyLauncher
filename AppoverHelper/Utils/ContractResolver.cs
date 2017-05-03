using Newtonsoft.Json.Serialization;

namespace AppoverHelper.Utils
{
    internal class ContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);
        }
    }
}