using System;
using System.Threading.Tasks;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Updater;
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
            Logging.TerminologyLogger.GetLogger().Info("Handling update application event!");

            try
            {
                if (this.Engine.UpdateManager.GetupdateInfo().UpdateType != UpdateType.Higher)
                {
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("Update",
                        "No update available. You are using the latest version.");
                    return;
                }

                var updateInfo = this.Engine.UpdateManager.GetupdateInfo();
                var confirm = this.Engine.UiControl.MainWindow.PopupConfirmDialog("Update",
                    $"Do you confirm to update from {this.Engine.UpdateManager.Version.CoreVersion}-{this.Engine.UpdateManager.Version.BuildNumber} to {updateInfo.LatestVersion.CoreVersion}-{updateInfo.LatestVersion.BuildNumber}");
                if (confirm != null && !confirm.Value)
                {
                    return;
                }


                var progress = new InternalNodeProgress("Update Application");
                var progressWindow = this.Engine.UiControl.MainWindow.BeginPopupProgressWindow(progress);
                Task.Run(() =>
                {
                    try
                    {
                        var message = this.Engine.UpdateManager.FetchLatestVersionAndStartUpdate(progress);
                        this.Engine.Exit();
                    }
                    catch (Exception ex)
                    {
                        Logging.TerminologyLogger.GetLogger()
                            .Error($"Cannot update because {ex}");
                        this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot launch",
                            $"Caused by an internal error, we cannot update right now. Detail: {ex.Message}");
                    }
                    finally
                    {
                        progressWindow.CrossThreadClose();
                    }
                });
                progressWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                TerminologyLogger.GetLogger()
                    .ErrorFormat($"Cannot update launcher because {ex}");
                this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot update",
                    $"Caused by an error, we cannot update launcher right now. Detail: {ex.Message}");

                throw;
            }
        }

        public override string Name => "UPDATE_APPLICATION";
    }
}