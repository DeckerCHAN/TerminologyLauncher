using System;
using System.Linq;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.Core.Handlers.InstanceCreator
{
    public class RemovePackFilehandler : HandlerBase
    {
        public override string Name => "CREATED_INSTANCE_REMOVE_PACK_FILE";

        public RemovePackFilehandler(Engine engine) : base(engine)
        {
           this.Engine.UiControl.InstanceCrateWindow.PackageFileRemoving+= this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            TerminologyLogger.GetLogger().Info(sender);
        }
    }
}