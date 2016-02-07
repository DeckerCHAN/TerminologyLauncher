using System;
using System.Collections.ObjectModel;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.Core.Handlers.MainHandlers
{
    public class RemoveInstanceHandler : HandlerBase
    {
        public RemoveInstanceHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.MainWindow.InstanceRemoveButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            Logging.TerminologyLogger.GetLogger().Info("Handling remove instance event!");

            var instanceName = this.Engine.UiControl.MainWindow.SelectInstance.InstanceName;
            try
            {
                var message = this.Engine.InstanceManager.RemoveInstance(instanceName);

                this.Engine.UiControl.MainWindow.InstanceList = new ObservableCollection<InstanceEntity>(this.Engine.InstanceManager.InstancesWithLocalImageSource);

            }
            catch (Exception ex)
            {
                TerminologyLogger.GetLogger()
                .ErrorFormat("Cannot remove this instance because {0}", ex);
                this.Engine.UiControl.MainWindow.PopupNotifyDialog("Cannot launch", String.Format("Caused by an error, we cannot remove this instance right now. Detail: {0}", ex.Message));

                throw;
            }
        }
    }
}
