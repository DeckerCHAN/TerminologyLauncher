using System;
using System.Runtime.CompilerServices;

namespace TerminologyLauncher.Core.Handlers.SystemHandlers
{
    public class CreateInstanceHandler:HandlerBase
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
                this.Engine.UiControl.StartInstanceCreateWindow(this.Engine.UiControl.MainWindow.SelectInstance);
            }
        }


    }
}