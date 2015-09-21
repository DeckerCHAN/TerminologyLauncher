using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Entities.InstanceManagement;

namespace TerminologyLauncher.Core.Handlers.MainHandlers
{
    public class RemoveInstanceHandler : HandlerBase
    {
        public RemoveInstanceHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.MajorWindow.InstanceRemoveButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            Logging.Logger.GetLogger().Info("Handling remove instance event!");

            var instanceName = this.Engine.UiControl.MajorWindow.SelectInstance.InstanceName;
            try
            {
                var message = this.Engine.InstanceManager.RemoveInstance(instanceName);

                this.Engine.UiControl.MajorWindow.InstanceList = new ObservableCollection<InstanceEntity>(this.Engine.InstanceManager.InstancesWithLocalImageSource);

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
