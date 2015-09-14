using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.GUI;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Core.Handlers.MainHandlers
{
    class LaunchInstanceHandler : HandlerBase
    {
        public LaunchInstanceHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.MajorWindow.LaunchInstanceButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            Logging.Logger.GetLogger().Info("Handling launch instance event!");

            InstanceEntity instance = this.Engine.UiControl.MajorWindow.SelectInstance;
            var progress = new InternalNodeProgress(String.Format("Launching instance {0}", instance.InstanceName));
            var progressWindow = new ProgressWindow(progress);
            Task.Run(() =>
            {
                try
                {
                    this.Engine.GameProcess = this.Engine.InstanceManager.LaunchInstance(progress, instance.InstanceName,
                        this.Engine.AuthServer.CurrentPlayer);
                    this.Engine.GameProcess.Exited += (s, o) =>
                    {
                        this.Engine.UiControl.ShowMainWindow();
                    };
                    this.Engine.UiControl.HideMainWindow();
                    this.Engine.GameProcess.BeginOutputReadLine();
                    this.Engine.GameProcess.OutputDataReceived += (s, ea) =>
                    {

                        if (!String.IsNullOrEmpty(ea.Data))
                        {
                            Logging.Logger.GetLogger().InfoFormat(">>>GAME<<<: {0}", ea.Data);
                        }
                    };
                }
                catch (WebException ex)
                {
                    var response = ((HttpWebResponse) ex.Response);
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.NotFound:
                        {
                            Logging.Logger.GetLogger()
                                .ErrorFormat("Can not find file on server when donloading:{0}", response.ResponseUri);
                            this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Can not launch",
                                String.Format(
                                    "Can not find file on server when donloading:{0}", response.ResponseUri));
                            break;
                        }
                        case HttpStatusCode.Forbidden:
                        {
                            Logging.Logger.GetLogger()
                             .ErrorFormat("You have no right to access this server when downloading: {0}", response.ResponseUri);
                            this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Can not launch",
                                String.Format(
                                    "You have no right to access this server when downloading: {0}", response.ResponseUri));
                 
                            break;
                        }
                        default:
                        {

                            Logging.Logger.GetLogger()
             .Error(String.Format("Encounter an network error during build environment: {0}", ex));
                            this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Can not launch", String.Format(
                                "Encounter an network error during build environment: {0}", ex.Message));
      
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Logger.GetLogger()
                        .Error(String.Format("Can not launch this instance because {0}", ex));
                    this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Can not launch", String.Format(
                        "Caused by an internal error, we can not launch this instance right now. Detail: {0}", ex.Message));
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
