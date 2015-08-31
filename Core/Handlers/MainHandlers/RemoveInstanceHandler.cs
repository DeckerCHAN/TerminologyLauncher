using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var instanceName = this.Engine.UiControl.MajorWindow.SelectInstance.InstanceName;
            this.Engine.InstanceManager.RemoveInstance(instanceName);
            this.Engine.UiControl.MajorWindow.InstanceList = this.Engine.InstanceManager.InstancesWithLocalImageSource.ToArray();
        }
    }
}
