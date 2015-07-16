using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.InstanceManager
{
    public class PlaceHolderReplacer
    {
        private Dictionary<String, String> Dictionary { get; set; }

        public PlaceHolderReplacer()
        {

        }

        public void AddToDictionary(String key, String value)
        {
            if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException("Key or value should not be null or empty!");
            }
            this.Dictionary.Add(key, value);
        }

        public void RemoveFromDictionary(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("Key or value should not be null or empty!");
            }
            this.Dictionary.Remove(key);
        }

        public String ReplaceArgument(String argument)
        {
            return this.Dictionary.Aggregate(argument, (current, kp) => current.Replace(kp.Key, kp.Value));
        }
    }
}
