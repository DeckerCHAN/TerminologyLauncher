using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Core.Handlers.MainHandlers
{
    class LaunchInstanceHandler : HandlerBase
    {
        public LaunchInstanceHandler(Engine engine)
            : base(engine)
        {

        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            InstanceEntity instance = null;
            var progress = new InternalNodeProgress(String.Format("Launching instance {0}", instance.InstanceName));
            this.Engine.InstanceManager.LaunchInstance(progress, instance.InstanceName, this.Engine.CurrentPlayer);
        }
    }
}
