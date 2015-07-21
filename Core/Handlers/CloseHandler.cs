using System;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.Core.Handlers
{
    public class CloseHandler:HandlerBase
    {
        public override void HandleEvent(object sender, EventArgs e)
        {
            Logger.GetLogger().Info("Handling close event.");
            Engine.GetEngine().UiControl.Shutdown();
            Logger.GetLogger().Info("UiControl shutdown.");
            Engine.GetEngine().Exit();
        }

        public CloseHandler(Engine engine) : base(engine)
        {
            this.Engine.UiControl.LoginWindow.CloseButton.Click += this.HandleEvent;
            this.Engine.UiControl.LoginWindow.CancleButton.Click += this.HandleEvent;
            //TODO:Add close button to main window
        }
    }
}
