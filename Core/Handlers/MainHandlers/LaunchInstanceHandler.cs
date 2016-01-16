using System;
using System.Net;
using System.Threading.Tasks;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Core.Handlers.MainHandlers
{
    class LaunchInstanceHandler : HandlerBase
    {
        public LaunchInstanceHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.MainWindow.LaunchInstanceButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            Logging.Logger.GetLogger().Info("Handling launch instance event!");

            var instance = this.Engine.UiControl.MainWindow.SelectInstance;
            var progress = new InternalNodeProgress(String.Format("Launching instance {0}", instance.InstanceName));
            var progressWindow = this.Engine.UiControl.MainWindow.BeginPopupProgressWindow(progress);
            Task.Run(() =>
            {
                try
                {
                    this.Engine.GameProcess = this.Engine.InstanceManager.LaunchInstance(progress, instance.InstanceName,
                        this.Engine.AuthServer.CurrentPlayer);
                    var usingConsole = this.Engine.CoreConfig.GetConfigObject<Boolean>("usingConsoleWindow");
                    this.Engine.GameProcess.Exited += (s, o) =>
                    {
                        this.Engine.UiControl.HideConsoleWindow();
                        this.Engine.UiControl.ShowMainWindow();
                    };
                    this.Engine.UiControl.HideMainWindow();
                    if (usingConsole)
                    {
                        this.Engine.UiControl.ConsoleWindow.Process = this.Engine.GameProcess;
                        this.Engine.UiControl.ShowConsoleWindow();

                    }
                }
                catch (WebException ex)
                {
                    var response = ((HttpWebResponse)ex.Response);
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                            {
                                Logging.Logger.GetLogger()
                                    .ErrorFormat("Cannot find file on server when donloading:{0}", response.ResponseUri);
                                this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot launch",
                                    String.Format(
                                        "Cannot find file on server when donloading:{0}", response.ResponseUri));
                                break;
                            }
                        case HttpStatusCode.Forbidden:
                            {
                                Logging.Logger.GetLogger()
                                 .ErrorFormat("You have no right to access this server when downloading: {0}", response.ResponseUri);
                                this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot launch",
                                    String.Format(
                                        "You have no right to access this server when downloading: {0}", response.ResponseUri));

                                break;
                            }
                        default:
                            {

                                Logging.Logger.GetLogger()
                 .Error(String.Format("Encounter an network error during build environment: {0}", ex));
                                this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot launch", String.Format(
                                    "Encounter an network error during build environment: {0}", ex.Message));

                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logger.GetLogger()
                        .Error(String.Format("Cannot launch this instance because {0}", ex));
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot launch", String.Format(
                        "Caused by an internal error, we cannot launch this instance right now. Detail: {0}", ex.Message));
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
