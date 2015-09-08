using System;
using System.Collections.Generic;
using System.Linq;
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
                }
                catch (Exception ex)
                {
                    Logging.Logger.GetLogger().Error(String.Format("Can not launch this instance cause {0}", ex.ToString()));
                    try
                    {
                        progressWindow.Close();
                    }
                    catch (Exception)
                    {
                        //ignore
                    }
                    new PopupWindow(this.Engine.UiControl.MajorWindow, "Can not launch",
                        "Caused by an internal error, we can not launch this instance right now.").ShowDialog();


                }
            });
            progressWindow.ShowDialog();

        }


    }
}
