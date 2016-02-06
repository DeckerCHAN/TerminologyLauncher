using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Utils.SerializeUtils;

namespace TerminologyLauncher.I18n
{
    static class GenerateTranslate
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var root = TranslationToolkits.GenerateFllTranslation();

            File.WriteAllText(Path.Combine(new DirectoryInfo(System.Reflection.Assembly.GetEntryAssembly().Location).Parent.Parent.Parent.FullName, "Translations", "FULL"), JsonConverter.ConvertToJson(root), Encoding.UTF8);
        }
    }
}
