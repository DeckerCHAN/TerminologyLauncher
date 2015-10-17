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
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Test
{
    class Program
    {
        [MTAThread]
        static void Main(string[] args)
        {
            DownloadUtils.GetWebContent(new LeafNodeProgress("Hello"), "http://www.baidu.com");

            Console.ReadKey();
        }
    }
}
