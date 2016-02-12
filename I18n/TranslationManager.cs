using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.I18n
{
    public sealed class TranslationManager
    {
        #region Instance

        private static TranslationManager Instance;

        public static TranslationManager TranslationProviderInstance
        {
            get
            {
                return Instance ?? (Instance = new TranslationManager());
            }
        }
        #endregion

        public String CurrentLanguageName { get; private set; }
        public FileInfo TranslatonFileInfo { get; private set; }
        public Dictionary<String, String> TranslationDictionary { get; private set; }

        private TranslationManager()
        {
            this.CurrentLanguageName = MachineUtils.GetCurrentLanguageName();
            this.TranslatonFileInfo = new FileInfo(Path.Combine(Assembly.GetEntryAssembly().Location, String.Format("{0}.ln", this.CurrentLanguageName)));
            if (!this.TranslatonFileInfo.Exists)
            {
                File.CreateText(this.TranslatonFileInfo.FullName);
            }
            this.LoadFile();
        }

        public String Localize(String identity, String defaultContent)
        {
            var key = String.Format("{0}.{1}", new StackTrace().GetFrame(1).GetMethod().Name, identity);
            if (this.TranslationDictionary.ContainsKey(key))
            {
                return this.TranslationDictionary[key];
            }
            else
            {
                this.TranslationDictionary.Add(key, defaultContent);
                this.SaveFile();
                return defaultContent;
            }
        }

        public void LoadFile()
        {
            this.TranslationDictionary = new Dictionary<string, string>();
            try
            {
                var lines = File.ReadAllLines(this.TranslatonFileInfo.FullName);

                if (lines.Length == 0) return;
                foreach (var line in lines)
                {
                    if (!line.Contains("="))
                    {
                        continue;
                    }
                    var key = line.Substring(0, line.IndexOf('='));
                    var value = line.Substring(line.IndexOf('='));
                    this.TranslationDictionary.Add(key, value);
                }
            }
            catch
            {
                this.TranslationDictionary = new Dictionary<string, string>();
            }
        }

        public void SaveFile()
        {
            using (var file = new StreamWriter(File.Open(this.TranslatonFileInfo.FullName, FileMode.Create)))
            {
                file.Write("#Terminology Translation File");
                var keys = this.TranslationDictionary.Keys.ToArray();
                Array.Sort(keys);
                foreach (var key in keys)
                {
                    file.WriteLine("{0}={1}", key, this.TranslationDictionary[key]);
                }
            }
        }
    }
}
