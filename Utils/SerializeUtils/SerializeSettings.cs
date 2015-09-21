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
