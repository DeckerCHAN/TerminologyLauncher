using System;
using Newtonsoft.Json;

namespace TerminologyLauncher.Entities.SerializeUtils
{
    public class JsonConverter
    {
        public static String ConvertToJson(Object obj)
        {
            return JsonConvert.SerializeObject(obj, new SerializeSettings());
        }

        public static T Parse<T>(String json)
        {
            return JsonConvert.DeserializeObject<T>(json, new SerializeSettings());
        }
    }
}
