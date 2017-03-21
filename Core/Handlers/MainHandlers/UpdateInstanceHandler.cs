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
            Logging.TerminologyLogger.GetLogger().Info("Handling update instance event!");

            var instance = this.Engine.UiControl.MainWindow.SelectInstance;
            if (instance == null)
            {
                Logging.TerminologyLogger.GetLogger().Warn("Did not select any instance. Ignore!");
                return;
            }

            var progress = new InternalNodeProgress($"Updating instance {instance.InstanceName}");
            var progressWindow = this.Engine.UiControl.MainWindow.BeginPopupProgressWindow(progress);
            Task.Run(() =>
            {
                try
                {
                    var message = this.Engine.InstanceManager.UpdateInstance(progress, instance.InstanceName);
                    progressWindow.CrossThreadClose();
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("Successful updated", message);
                    this.Engine.UiControl.MainWindow.InstanceList =
                        new ObservableCollection<InstanceEntity>(
                            this.Engine.InstanceManager.InstancesWithLocalImageSource);
                }
                catch (NoAvailableUpdateException ex)
                {
                    Logging.TerminologyLogger.GetLogger().Info(ex.Message);
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("No available update", ex.Message);
                }
                catch (WrongStateException ex)
                {
                    Logging.TerminologyLogger.GetLogger()
                        .ErrorFormat($"Update instance {instance.InstanceName} encountered an error: {ex.Message}");
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot update",
                        $"Encounter an wrong state error. Detail:{ex.Message}");
                }
                catch (Exception ex)
                {
                    Logging.TerminologyLogger.GetLogger()
                        .ErrorFormat($"Update instance {instance.InstanceName} encountered an error:\n{ex}");
                    this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot update",
                        $"Caused by an internal error, we cannot update this instance right now.Detail:{ex.Message}");
                }
                finally
                {
                    progressWindow.CrossThreadClose();
                }
            });
            progressWindow.ShowDialog();
        }

        public override string Name => "UPDATE_INSTANCE";
    }
}