using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TerminologyLauncher.Configs.Exceptions;
using TerminologyLauncher.Utils;

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
                throw new FileNotFoundException($"Config file {jsonConfigFile} not exists!");
            }
            this.JsonFileInfo = jsonConfigFile;
        }

        public string GetConfigString(string key)
        {
            this.ReadConfigsFromFile();
            this.CheckKey(key);
            var value = this.ConfigJObject.SelectToken(key);
            if (value == null)
            {
                try
                {
                    var ass = Assembly.GetCallingAssembly();
                    var resourceAccessString = ass.GetManifestResourceNames().First(x => x.EndsWith(this.JsonFileInfo.Name));
                    var stream = ResourceUtils.ReadEmbedFileResource(ass, resourceAccessString);
                    var obj = JObject.Parse(new StreamReader(stream).ReadToEnd());
                    var newValue = obj.SelectToken(key);
                    if (newValue == null)
                    {
                        throw new Exception();
                    }
                    this.SetConfigString(key, newValue.ToString());
                    return newValue.ToString();
                }
                catch (Exception)
                {
                    throw new ConfigurationKeyNotFoundException(this.JsonFileInfo.Name, key);
                }

            }
            else
            {
                return value.ToString();
            }
        }


        public void SetConfigString(string key, string value)
        {
            this.ReadConfigsFromFile();
            this.CheckKey(key);
            var token = this.ConfigJObject.SelectToken(key);
            if (token != null)
            {
                token.Replace(new JValue(value));
            }
            else
            {
                this.ConfigJObject.Add(new JProperty(key, value));
            }

            this.SaveConfig();



        }

        public T GetConfigObject<T>(string key)
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

        public void SetConfigObject(string key, object obj)
        {
            this.ReadConfigsFromFile();
            this.CheckKey(key);
            this.ConfigJObject.SelectToken(key).Replace(JsonConvert.SerializeObject(obj));
            this.SaveConfig();
        }

        private string GetUpperKey(string key)
        {
            return char.ToLowerInvariant(key[0]) + key.Substring(1);
        }

        private void CheckKey(string key)
        {
            if (string.IsNullOrEmpty(key))
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
