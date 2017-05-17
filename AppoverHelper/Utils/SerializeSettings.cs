using Newtonsoft.Json;

namespace AppoverHelper.Utils
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