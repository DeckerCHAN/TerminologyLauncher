using System;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.GUI;
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
            this.Engine.UiControl.MajorWindow.InstanceAddButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            Logger.GetLogger().Info("Handling add instance event!");

            var result = new SingleLineInputWindow("Input instance url", "URL:").ReceiveUserInput();
            if (result.Type == WindowResultType.Canceled)
            {
                Logger.GetLogger().Info("Empty input or user canceled. Ignore!");
                return;
            }
            try
            {
                var message = this.Engine.InstanceManager.AddInstance(result.Result.ToString());
                this.Engine.UiControl.MajorWindow.InstanceList =
                new ObservableCollection<InstanceEntity>(this.Engine.InstanceManager.InstancesWithLocalImageSource);
                this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Successful", message);
            }
            catch (WebException ex)
            {
                Logger.GetLogger()
                       .ErrorFormat("Network is not accessable! Detail: {0}", ex.Message);
                this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Error", String.Format("Network is not accessable! Detail: {0}", ex.Message));
            }
            catch (JsonReaderException ex)
            {
                Logger.GetLogger()
                    .ErrorFormat("Wrong instance json format! {0}", ex.Message);
                this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Error", String.Format("Wrong instance json format! {0}", ex.Message));
            }
            catch (MissingFieldException)
            {
                new PopupWindow(this.Engine.UiControl.MajorWindow, "Error",
                    "Some critical field is missing. Unable to add this instance.!").ShowDialog();
            }

            catch (Exception ex)
            {
                Logger.GetLogger()
                    .ErrorFormat("Can not add this instance because {0}", ex);
                this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Can not launch", String.Format(
                    "Caused by an error, we can not add this instance right now. Detail: {0}", ex.Message));

            }
            finally
            {
         
            }


        }
    }
}
