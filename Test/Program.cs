using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.GUI;
using TerminologyLauncher.GUI.ToolkitWindows.SingleSelect;
using TerminologyLauncher.I18n;
using TerminologyLauncher.I18n.TranslationObjects;
using TerminologyLauncher.I18n.TranslationObjects.GUITranslations;
using TerminologyLauncher.I18n.TranslationObjects.HandlerTranslations;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.ProgressService;
using TerminologyLauncher.Utils.SerializeUtils;

namespace TerminologyLauncher.Test
{
    class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            var root = TranslationToolkits.GenerateFllTranslation();

            File.WriteAllText("G:\\CSharp\\TerminologyLauncher\\I18n\\Translations\\FULL", JsonConverter.ConvertToJson(root), Encoding.UTF8);
        }
    }
}
