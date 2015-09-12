using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.GUI;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Core.Handlers.MainHandlers
{
    public class UpdateApplicationHandler : HandlerBase
    {
        public UpdateApplicationHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.MajorWindow.UpdateButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            var progress = new InternalNodeProgress("Update Application");
            var progressWindow = new ProgressWindow(progress);
            Task.Run(() =>
            {
                try
                {
                    var message = this.Engine.UpdateManager.FetchLatestVersionAndStartUpdate(progress);
                    this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Update", message);
                }
                catch (Exception ex)
                {

                    Logging.Logger.GetLogger()
                            .Error(String.Format("Can not update because {0}", ex.Message));
                    this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Can not launch", String.Format(
                        "Caused by an internal error, we can not update right now. Detail: {0}", ex));
                }
                finally
                {
                    progressWindow.CrossThreadClose();
                }
            });
            progressWindow.ShowDialog();
        }
    }
}
