using Newtonsoft.Json;

namespace TerminologyLauncher.Utils.SerializeUtils
{
    internal class SerializeSettings : JsonSerializerSettings
    {
        public SerializeSettings()
        {
            this.ContractResolver = new ContractResolver();
            this.NullValueHandling = NullValueHandling.Ignore;
            this.Formatting = Formatting.Indented;
        }
    }
}