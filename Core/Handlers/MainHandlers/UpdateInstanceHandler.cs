using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.GUI;
using TerminologyLauncher.InstanceManagerSystem.Exceptions;
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
                    var message = this.Engine.InstanceManager.UpdateInstance(progress, instance.InstanceName);
                    Logging.Logger.GetLogger().InfoFormat(message);
                    this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Successful updated", message);

                }
                catch (NoAvailableUpdateException ex)
                {
                    Logging.Logger.GetLogger().Info(ex.Message);
                    this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "No available update",
                        ex.Message);
                }
                catch (WrongStateException ex)
                {
                    Logging.Logger.GetLogger().ErrorFormat("Update instance {0} encountered an error:\n{1}", instance.InstanceName, ex.Message);
                    this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Can not update",
                        String.Format(
                            "Encounter an wrong state error. Detail:{0}",
                            ex.Message));

                }
                catch (Exception ex)
                {
                    Logging.Logger.GetLogger().ErrorFormat("Update instance {0} encountered an error:\n{1}", instance.InstanceName, ex.Message);
                    this.Engine.UiControl.StartPopupWindow(this.Engine.UiControl.MajorWindow, "Can not update",
                        String.Format(
                            "Caused by an internal error, we can not update this instance right now.Detail:{0}",
                            ex.Message));

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
