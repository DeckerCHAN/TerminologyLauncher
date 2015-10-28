using System;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.GUI;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.GUI.ToolkitWindows;
using TerminologyLauncher.GUI.ToolkitWindows.PopupWindow;
using TerminologyLauncher.GUI.ToolkitWindows.SingleLineInput;
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
            Logger.GetLogger().Info("Handling add instance event!");

            var result = new FieldReference<String>(String.Empty);

            var confim =
                this.Engine.UiControl.MainWindow.PopupSingleLineInputDialog(
                    I18n.TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation
                        .InputInstanceUrlTranslation.InputWindowTitleTranslation,
                    I18n.TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation
                        .InputInstanceUrlTranslation.InputFieldTranslation, result);
            if (confim == null || confim.Value)
            {
                Logger.GetLogger().Info("Empty input or user canceled. Ignore!");
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
                Logger.GetLogger()
                       .ErrorFormat("Network is not accessable! Detail: {0}", ex.Message);
                this.Engine.UiControl.MainWindow.PopupNotifyDialog("Error", String.Format("Network is not accessable! Detail: {0}", ex.Message));
            }
            catch (JsonReaderException ex)
            {
                Logger.GetLogger()
                    .ErrorFormat("Wrong instance json format! {0}", ex.Message);
                this.Engine.UiControl.MainWindow.PopupNotifyDialog("Error", String.Format("Wrong instance json format! {0}", ex.Message));
            }
            catch (MissingFieldException)
            {
                this.Engine.UiControl.MainWindow.PopupNotifyDialog("Error",
                    "Some critical field is missing. Unable to add this instance.!");
            }

            catch (Exception ex)
            {
                Logger.GetLogger()
                    .ErrorFormat("Can not add this instance because {0}", ex);
                this.Engine.UiControl.MainWindow.PopupNotifyDialog("Can not launch", String.Format("Caused by an error, we can not add this instance right now. Detail: {0}", ex.Message));

            }
            finally
            {

            }


        }
    }
}
