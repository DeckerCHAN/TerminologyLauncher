using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.SerializeUtils;
using TerminologyLauncher.GUI;
using TerminologyLauncher.GUI.SingleLineInputWindow;

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


            var result = new SingleLineInputWindow("Input url pls!", "url").ReceiveUserinput();
            if (result.Type == SingleLineInputResultType.Canceled)
            {
                Logging.Logger.GetLogger().Info("Empty input or user canceled. Ignore!");
                return;
            }
            try
            {
                this.Engine.InstanceManager.AddInstance(result.InputLine);

            }
            catch (WebException)
            {

                new PopupWindow(this.Engine.UiControl.MajorWindow, "Error", "Network not accessible!").ShowDialog();
                return;
            }
            catch (FormatException)
            {
                new PopupWindow(this.Engine.UiControl.MajorWindow, "Error", "Wrong instance format!").ShowDialog();
                return;
            }
            finally
            {
                this.Engine.UiControl.MajorWindow.InstanceList = this.Engine.InstanceManager.InstancesWithLocalImageSource.ToArray();

            }


        }
    }
}
