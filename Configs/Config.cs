using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace TerminologyLauncher.Configs
{
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

        public String GetConfig(String key)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Empty key is not allowed!");
            }
            this.ReadConfigsFromFile();
            return this.ConfigJObject.SelectToken(key).ToString();
        }


        public void SetConfig(String key, String value)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Empty key is not allowed!");
            }

            if (this.ConfigJObject.SelectToken(key) != null)
            {
                this.ConfigJObject.SelectToken(key).Replace(new JValue(value));
                this.SaveConfig();
            }
            else
            {
                throw new KeyNotFoundException(String.Format("Can not find exist key:{0}. Thus, unable to set that value.", key));
            }


        }

        private String GetUpperKey(String key)
        {
            return Char.ToLowerInvariant(key[0]) + key.Substring(1);
        }

        private void SaveConfig()
        {
            using (var configFileStream = new FileStream(this.JsonFileInfo.FullName, FileMode.Open))
            {
                using (var sw = new StreamWriter(configFileStream))
                {
                    sw.Write(this.ConfigJObject.ToString());

                }
            }
        }

        private void ReadConfigsFromFile()
        {
            using (var configFileStream = new FileStream(this.JsonFileInfo.FullName, FileMode.Open))
            {
                using (var sr = new StreamReader(configFileStream))
                {
                    this.ConfigJObject = JObject.Parse(sr.ReadToEnd());
                }
            }
        }


    }
}
