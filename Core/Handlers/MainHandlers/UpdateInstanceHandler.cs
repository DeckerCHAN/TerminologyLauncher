using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.InstanceManagerSystem.Exceptions;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Core.Handlers.MainHandlers
{
    class UpdateInstanceHandler : HandlerBase
    {
        public UpdateInstanceHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.MainWindow.UpdateInstanceButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            Logging.Logger.GetLogger().Info("Handling update instance event!");

            var instance = this.Engine.UiControl.MainWindow.SelectInstance;
            if (instance == null)
            {
                Logging.Logger.GetLogger().Warn("Did not select any instance. Ignore!");
                return;
            }

            var progress = new InternalNodeProgress(String.Format("Updating instance {0}", instance.InstanceName));
            var progressWindow = this.Engine.UiControl.MainWindow.BeginPopupProgressWindow(progress);
            Task.Run(() =>
            {
                try
                {
                    var message = this.Engine.InstanceManager.UpdateInstance(progress, instance.InstanceName);
                    progressWindow.CrossThreadClose();
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("Successful updated", message);
                    this.Engine.UiControl.MainWindow.InstanceList = new ObservableCollection<InstanceEntity>(this.Engine.InstanceManager.InstancesWithLocalImageSource);
                }
                catch (NoAvailableUpdateException ex)
                {
                    Logging.Logger.GetLogger().Info(ex.Message);
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("No available update", ex.Message);
                }
                catch (WrongStateException ex)
                {
                    Logging.Logger.GetLogger().ErrorFormat("Update instance {0} encountered an error: {1}", instance.InstanceName, ex.Message);
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("Can not update",
                        String.Format(
                            "Encounter an wrong state error. Detail:{0}",
                            ex.Message));

                }
                catch (Exception ex)
                {
                    Logging.Logger.GetLogger().ErrorFormat("Update instance {0} encountered an error:\n{1}", instance.InstanceName, ex);
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("Can not update",
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
