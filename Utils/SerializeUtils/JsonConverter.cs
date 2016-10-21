using System;
using Newtonsoft.Json;

namespace TerminologyLauncher.Utils.SerializeUtils
{
    public class JsonConverter
    {
        public static string ConvertToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, new SerializeSettings());
        }

        public static T Parse<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new SerializeSettings());
        }
    }
}