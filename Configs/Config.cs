using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TerminologyLauncher.Configs.Exceptions;

namespace TerminologyLauncher.Configs
{
    //TODO:Using reflect to find default config if config is missing in file
    public class Config
    {
        public FileInfo JsonFileInfo { get; protected set; }
        public JObject ConfigJObject { get; protected set; }
        public Config(FileInfo jsonConfigFile)
        {
            if (!jsonConfigFile.Exists)
            {
                throw new FileNotFoundException(String.Format("Config file {0} not exists!", jsonConfigFile));
            }
            this.JsonFileInfo = jsonConfigFile;
        }

        public String GetConfigString(String key)
        {
            this.ReadConfigsFromFile();
            this.CheckKey(key);
            var value = this.ConfigJObject.SelectToken(key);
            if (value == null)
            {
                throw new ConfigurationKeyNotFoundException(this.JsonFileInfo.Name, key);
            }
            else
            {
                return value.ToString();
            }
        }

        public void SetMultiConfigString(String key, List<String> configs)
        {
            this.ReadConfigsFromFile();
            this.CheckKey(key);
            var jarray = new JArray();
            foreach (var config in configs)
            {
                jarray.Add(new JValue(config));
            }
            this.ConfigJObject.SelectToken(key).Replace(jarray);
            this.SaveConfig();
        }

        public IEnumerable<string> GetMultiConfigString(String key)
        {
            this.ReadConfigsFromFile();
            this.CheckKey(key);
            var rowList = this.ConfigJObject.SelectToken(key);

            if (rowList == null)
            {
                throw new ConfigurationKeyNotFoundException(this.JsonFileInfo.Name, key);
            }
            else
            {
                var value = rowList.ToObject<List<String>>();
                return value.Count == 0 ? new List<string>() : value;
            }

            
        }


        public void SetConfigString(String key, String value)
        {
            this.ReadConfigsFromFile();
            this.CheckKey(key);
            this.ConfigJObject.SelectToken(key).Replace(new JValue(value));
            this.SaveConfig();



        }

        public T GetConfigObject<T>(String key)
        {
            this.ReadConfigsFromFile();
            this.CheckKey(key);
            var rowData = this.ConfigJObject.SelectToken(key);
            if (rowData == null)
            {
                throw new ConfigurationKeyNotFoundException(this.JsonFileInfo.Name, key);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(rowData.ToString()); 
            }
            
        }

        public void SetConfigObject(String key, Object obj)
        {
            this.ReadConfigsFromFile();
            this.CheckKey(key);
            this.ConfigJObject.SelectToken(key).Replace(JsonConvert.SerializeObject(obj));
            this.SaveConfig();
        }

        private String GetUpperKey(String key)
        {
            return Char.ToLowerInvariant(key[0]) + key.Substring(1);
        }

        private void CheckKey(String key)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Empty key is not allowed!");
            }
            //if (this.ConfigJObject.SelectToken(key) == null)
            //{
            //    throw new KeyNotFoundException(String.Format("The key {0} is not exists in config!", key));
            //}
        }

        private void SaveConfig()
        {
            Monitor.Enter(this);
            File.WriteAllText(this.JsonFileInfo.FullName, this.ConfigJObject.ToString());
            Monitor.Exit(this);
        }

        private void ReadConfigsFromFile()
        {
            Monitor.Enter(this);
            this.ConfigJObject = JObject.Parse(File.ReadAllText(this.JsonFileInfo.FullName));
            Monitor.Exit(this);
        }


    }
}
