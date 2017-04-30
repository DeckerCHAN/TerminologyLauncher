using System;
using System.Linq;
using System.Runtime.CompilerServices;
using TerminologyLauncher.Entities.InstanceManagement;

namespace TerminologyLauncher.Core.Handlers.SystemHandlers
{
    public class CreateInstanceHandler : HandlerBase
    {
        public override string Name => "CREATE_INSTANCE";

        public CreateInstanceHandler(Engine engine) : base(engine)
        {
            this.Engine.UiControl.MainWindow.CreateInstanceButton.Click += this.HandleEvent;
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            if (this.Engine.UiControl.MainWindow.SelectInstance == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                var selected = this.Engine.UiControl.MainWindow.SelectInstance;
                var copy = Utils.CopyUtils.MakeCopy(this.Engine.InstanceManager.Instances
                    .First(x => x.InstanceName.Equals(selected.InstanceName)));
                this.Engine.UiControl.ShowInstanceCreateWindow();
                this.Engine.UiControl.InstanceCrateWindow.LoadInstnace((InstanceEntity)copy);
            }
        }
    }
}