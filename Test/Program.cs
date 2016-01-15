using System;
using System.IO;
using System.Text;
using TerminologyLauncher.I18n;
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
