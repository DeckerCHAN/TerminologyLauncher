using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.I18n
{
    public sealed class TranslationManager
    {
        #region Instance

        private static TranslationManager Instance;

        public static TranslationManager GetManager
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
            this.TranslatonFileInfo = new FileInfo(Path.Combine(new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName, "Translations", String.Format("{0}.ln", this.CurrentLanguageName)));
            if (!this.TranslatonFileInfo.Exists)
            {
                FolderUtils.CreateDirectoryIfNotExists(this.TranslatonFileInfo.Directory);
                File.CreateText(this.TranslatonFileInfo.FullName).Close();
            }
            this.LoadFile();
        }

        public String Localize(String identity, String defaultContent, UInt16 argumentNumber = 0)
        {
            if (!new Regex("^[a-zA-Z]+$").Match(identity).Success)
            {
                throw new ArgumentException("Identity not allow string contains characters except letters.");
            }
            var key = String.Format("{0}.{1}", new StackTrace().GetFrame(1).GetMethod().ReflectedType.FullName, identity);

            if (this.TranslationDictionary.ContainsKey(key))
            {
                if (new Regex("{.*?}").Match(this.TranslationDictionary[key]).Captures.Count == argumentNumber)
                {
                    return this.TranslationDictionary[key];
                }
                else
                {
                    this.TranslationDictionary[key] = defaultContent;
                }
            }
            else
            {
                this.TranslationDictionary.Add(key, defaultContent);
            }

            this.SaveFile();
            return defaultContent;

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
                    if (!line.Contains("=") || line.StartsWith("#"))
                    {
                        continue;
                    }
                    var key = line.Substring(0, line.IndexOf('='));
                    var value = line.Substring(line.IndexOf('=') + 1);
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
            using (var file = File.Open(this.TranslatonFileInfo.FullName, FileMode.Create))
            {
                using (var writer = new StreamWriter(file))
                {
                    writer.WriteLine("#Terminology Translation File");
                    var keys = this.TranslationDictionary.Keys.ToArray();
                    Array.Sort(keys);
                    foreach (var key in keys)
                    {
                        writer.WriteLine("{0}={1}", key, this.TranslationDictionary[key]);
                    }
                }
            }

        }
    }
}
