using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.SerializeUtils;
using TerminologyLauncher.GUI;
using TerminologyLauncher.GUI.ToolkitWindows.SingleSelect;
using TerminologyLauncher.I18n.TranslationObjects;
using TerminologyLauncher.I18n.TranslationObjects.GUITranslations;
using TerminologyLauncher.I18n.TranslationObjects.HandlerTranslations;

namespace TerminologyLauncher.Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            CultureInfo ci = CultureInfo.InstalledUICulture;

            Console.WriteLine("Default Language Info:");
            Console.WriteLine("* Name: {0}", ci.Name);
            Console.WriteLine("* Display Name: {0}", ci.DisplayName);
            Console.WriteLine("* English Name: {0}", ci.EnglishName);
            Console.WriteLine("* 2-letter ISO Name: {0}", ci.TwoLetterISOLanguageName);
            Console.WriteLine("* 3-letter ISO Name: {0}", ci.ThreeLetterISOLanguageName);
            Console.WriteLine("* 3-letter Win32 API Name: {0}", ci.ThreeLetterWindowsLanguageName);

            Console.ReadKey();
        }
    }
}
