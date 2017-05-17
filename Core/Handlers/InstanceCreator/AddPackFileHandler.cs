using System;
using System.Linq;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.Core.Handlers.InstanceCreator
{
    public class AddPackFilehandler : HandlerBase
    {
        public override string Name => "CREATED_INSTANCE_ADD_PACK_FILE";

        public AddPackFilehandler(Engine engine) : base(engine)
        {
            //this.Engine.UiControl.InstanceCrateWindow.AddNewPackageFileButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            TerminologyLogger.GetLogger().Info(sender);
        }
    }
}