using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.GUI;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Core.Handlers.MainHandlers
{
    class UpdateInstanceHandler : HandlerBase
    {
        public UpdateInstanceHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.MajorWindow.UpdateInstanceButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            var instance = this.Engine.UiControl.MajorWindow.SelectInstance;
            if (instance == null)
            {
                Logging.Logger.GetLogger().Warn("Did not select any instance. Ignore!");
                return;
            }

            var progress = new InternalNodeProgress(String.Format("Launching instance {0}", instance.InstanceName));
            var progressWindow = new ProgressWindow(progress);
            Task.Run(() =>
            {
                try
                {
                    this.Engine.InstanceManager.UpdateInstance(progress, instance.InstanceName);
                    Logging.Logger.GetLogger().InfoFormat("Successful updated instance {0}.", instance.InstanceName);
                }
                catch (Exception ex)
                {
                    Logging.Logger.GetLogger().ErrorFormat("Update instance {0} encountered an error:\n{1}", instance.InstanceName, ex);
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
