using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TerminologyLauncher.Configs
{
    public class Config
    {
        public FileInfo JsonFileInfo { get; protected set; }
        public JObject ConfigJObject { get; protected set; }
        public Config(FileInfo jsonConfigFile)
        {
            this.JsonFileInfo = jsonConfigFile;



        }

        public String GetConfig(String key)
        {
            this.ReadConfigsFromFile();
            return this.ConfigJObject.SelectToken(key).ToString();
        }


        public void SetConfig(String key, String value)
        {
            this.ConfigJObject.SelectToken(key).Replace(new JValue(value));
            this.SaveConfig();
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
