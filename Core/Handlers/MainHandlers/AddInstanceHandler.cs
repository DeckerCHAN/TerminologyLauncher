using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.SerializeUtils;
using TerminologyLauncher.GUI;
using TerminologyLauncher.GUI.SingleLineInput;

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


            var result = new SingleLineInputWindow("Input instance url", "URL:").ReceiveUserinput();
            if (result.Type == SingleLineInputResultType.Canceled)
            {
                Logging.Logger.GetLogger().Info("Empty input or user canceled. Ignore!");
                return;
            }
            try
            {
                var message = this.Engine.InstanceManager.AddInstance(result.InputLine);
                this.Engine.UiControl.MajorWindow.InstanceList =
                new ObservableCollection<InstanceEntity>(this.Engine.InstanceManager.InstancesWithLocalImageSource);
                this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Successful", message);
            }
            catch (WebException ex)
            {
                Logging.Logger.GetLogger()
                       .ErrorFormat("Network is not accessable! Detail: {0}", ex.Message);
                this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Error", String.Format("Network is not accessable! Detail: {0}", ex.Message));
            }
            catch (JsonReaderException ex)
            {
                Logging.Logger.GetLogger()
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
                Logging.Logger.GetLogger()
                    .ErrorFormat("Can not add this instance because {0}", ex);
                this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Can not launch", String.Format(
                    "Caused by an internal error, we can not launch this instance right now. Detail: {0}", ex.Message));

            }
            finally
            {
                this.Engine.UiControl.MajorWindow.InstanceList =
                    new ObservableCollection<InstanceEntity>(this.Engine.InstanceManager.InstancesWithLocalImageSource);

            }


        }
    }
}
