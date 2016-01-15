using System;
using System.Threading.Tasks;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Core.Handlers.SystemHandlers
{
    public class UpdateApplicationHandler : HandlerBase
    {
        public UpdateApplicationHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.MainWindow.UpdateButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            Logging.Logger.GetLogger().Info("Handling update application event!");

            var progress = new InternalNodeProgress("Update Application");
            var progressWindow = this.Engine.UiControl.MainWindow.BeginPopupProgressWindow(progress);
            Task.Run(() =>
            {
                try
                {
                    var message = this.Engine.UpdateManager.FetchLatestVersionAndStartUpdate(progress);
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("Update", message);
                }
                catch (Exception ex)
                {

                    Logging.Logger.GetLogger()
                            .Error(String.Format("Can not update because {0}", ex));
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("Can not launch", String.Format(
                        "Caused by an internal error, we can not update right now. Detail: {0}", ex.Message));
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
