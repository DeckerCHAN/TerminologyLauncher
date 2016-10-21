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
            Logging.TerminologyLogger.GetLogger().Info("Handling launch instance event!");

            var instance = this.Engine.UiControl.MainWindow.SelectInstance;
            var progress = new InternalNodeProgress($"Launching instance {instance.InstanceName}");
            var progressWindow = this.Engine.UiControl.MainWindow.BeginPopupProgressWindow(progress);
            Task.Run(() =>
            {
                try
                {
                    this.Engine.GameProcess = this.Engine.InstanceManager.LaunchInstance(progress, instance.InstanceName,
                        this.Engine.AuthServer.CurrentPlayer);
                    var usingConsole = this.Engine.CoreConfig.GetConfigObject<bool>("usingConsoleWindow");
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
                    var response = ((HttpWebResponse) ex.Response);
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                        {
                            Logging.TerminologyLogger.GetLogger()
                                .ErrorFormat($"Cannot find file on server when donloading:{response.ResponseUri}");
                            this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot launch",
                                $"Cannot find file on server when donloading:{response.ResponseUri}");
                            break;
                        }
                        case HttpStatusCode.Forbidden:
                        {
                            Logging.TerminologyLogger.GetLogger()
                                .ErrorFormat(
                                    $"You have no right to access this server when downloading: {response.ResponseUri}");
                            this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot launch",
                                $"You have no right to access this server when downloading: {response.ResponseUri}");

                            break;
                        }
                        default:
                        {
                            Logging.TerminologyLogger.GetLogger()
                                .Error($"Encounter an network error during build environment: {ex}");
                            this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot launch",
                                $"Encounter an network error during build environment: {ex.Message}");

                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.TerminologyLogger.GetLogger()
                        .Error($"Cannot launch this instance because {ex}");
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot launch",
                        $"Caused by an internal error, we cannot launch this instance right now. Detail: {ex.Message}");
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