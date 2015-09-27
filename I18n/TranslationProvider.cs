using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.SerializeUtils;
using TerminologyLauncher.I18n.TranslationObjects;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.I18n
{
    public class TranslationProvider
    {
        #region Instance

        private static TranslationProvider Instance;

        public static TranslationProvider TranslationProviderInstance
        {
            get
            {
                return Instance ?? (Instance = new TranslationProvider());
            }
        }

        #endregion

        private TranslationRoot TranslationObjectValue;
        public TranslationRoot TranslationObject
        {
            get
            {
                if (this.TranslationObjectValue != null) return this.TranslationObjectValue;
                var translationStream =
                    ResourceUtils.ReadEmbedFileResource("TerminologyLauncher.I18n.Translations." +
                                                        this.Config.GetConfigString("usingTranslation"));
                var translationContent = new StreamReader(translationStream,Encoding.UTF8).ReadToEnd();
                this.TranslationObjectValue = JsonConverter.Parse<TranslationRoot>(translationContent);

                return this.TranslationObjectValue;

            }
        }

        public Config Config { get; private set; }

        private TranslationProvider()
        {
            this.Config = new Config(new FileInfo("Configs/I18N.json"));
        }
    }
}
