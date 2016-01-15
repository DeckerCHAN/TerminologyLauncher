using System;
using System.IO;
using System.Reflection;
using System.Text;
using TerminologyLauncher.I18n.TranslationObjects;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.SerializeUtils;

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
                return this.TranslationObjectValue;
            }
        }

        private String UsingTranslation;

        private TranslationProvider()
        {
            this.UsingTranslation = MachineUtils.GetCurrentLanguageName();
            var translationStream =
                    ResourceUtils.ReadEmbedFileResource("TerminologyLauncher.I18n.Translations." +
                                                        this.UsingTranslation) ?? ResourceUtils.ReadEmbedFileResource("TerminologyLauncher.I18n.Translations." +
                                                        "en-US");
            var translationContent = new StreamReader(translationStream, Encoding.UTF8).ReadToEnd();
            this.TranslationObjectValue = JsonConverter.Parse<TranslationRoot>(translationContent);
            this.FillFieldValue(this.TranslationObjectValue);
        }

        private void FillFieldValue(Object obj)
        {

            var proprieties =
                obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var propertyInfo in proprieties)
            {
                if (propertyInfo.PropertyType == String.Empty.GetType())
                {
                    if (propertyInfo.GetValue(obj) == null)
                    {
                        propertyInfo.SetValue(obj, propertyInfo.Name);

                    }
                }
                else
                {
                    var valueObj = propertyInfo.GetValue(obj);
                    if (valueObj == null)
                    {
                        valueObj = Activator.CreateInstance(propertyInfo.PropertyType);
                    }
                    this.FillFieldValue(valueObj);
                    propertyInfo.SetValue(obj, valueObj);
                }
            }
        }
    }
}
