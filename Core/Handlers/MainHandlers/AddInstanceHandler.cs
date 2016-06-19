using System;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.I18n;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.Core.Handlers.MainHandlers
{
    public class AddInstanceHandler : HandlerBase
    {
        public AddInstanceHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.MainWindow.InstanceAddButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            TerminologyLogger.GetLogger().Info("Handling add instance event!");

            var result = new FieldReference<string>(string.Empty);

            var confim =
                this.Engine.UiControl.MainWindow.PopupSingleLineInputDialog(TranslationManager.GetManager.Localize("Title", "Add a Instance"),
                    TranslationManager.GetManager.Localize("InstanceUrl", "Instance URL:"), result);
            if (confim == null || confim.Value == false)
            {
                TerminologyLogger.GetLogger().Info("Empty input or user canceled. Ignore!");
                return;
            }
            try
            {
                var message = this.Engine.InstanceManager.AddInstance(result.Value);
                this.Engine.UiControl.MainWindow.InstanceList =
                new ObservableCollection<InstanceEntity>(this.Engine.InstanceManager.InstancesWithLocalImageSource);
                this.Engine.UiControl.MainWindow.PopupNotifyDialog("Successful", message);
            }
            catch (WebException ex)
            {
                TerminologyLogger.GetLogger()
                       .ErrorFormat("Network is not accessable! Detail: {0}", ex.Message);
                this.Engine.UiControl.MainWindow.PopupNotifyDialog(TranslationManager.GetManager.Localize("Error", "Error"), string.Format(TranslationManager.GetManager.Localize("NetworkNotAccessable", "Network is not accessable! Detail: {0}", 1), ex.Message));
            }
            catch (JsonReaderException ex)
            {
                TerminologyLogger.GetLogger()
                    .ErrorFormat("Wrong instance json format! {0}", ex.Message);
                this.Engine.UiControl.MainWindow.PopupNotifyDialog(TranslationManager.GetManager.Localize("Error", "Error"), string.Format(TranslationManager.GetManager.Localize("WrongJsonFormat", "Wrong instance json format! {0}", 1), ex.Message));
            }
            catch (MissingFieldException ex)
            {
                TerminologyLogger.GetLogger()
                  .ErrorFormat("Critical field is missing! {0}", ex.Message);
                this.Engine.UiControl.MainWindow.PopupNotifyDialog(TranslationManager.GetManager.Localize("Error", "Error"), string.Format(TranslationManager.GetManager.Localize("MissCriticalField", "Some critical field is missing. Unable to add this instance! Detail:{0}", 1), ex.Message));
            }

            catch (Exception ex)
            {
                TerminologyLogger.GetLogger()
                    .ErrorFormat("Cannot add this instance because {0}", ex);
                this.Engine.UiControl.MainWindow.PopupNotifyDialog(TranslationManager.GetManager.Localize("Error", "Error"), string.Format(TranslationManager.GetManager.Localize("UnknownError", "Caused by an error, we cannot add this instance right now. Detail: {0}", 1), ex.Message));

            }
        }
    }
}
