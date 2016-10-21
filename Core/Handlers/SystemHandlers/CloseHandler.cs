using System;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.Core.Handlers.SystemHandlers
{
    public class CloseHandler : HandlerBase
    {
        public override void HandleEvent(object sender, EventArgs e)
        {
            TerminologyLogger.GetLogger().Info("Handling close event.");

            Engine.GetEngine().Exit();
        }

        public CloseHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.LoginWindow.CloseButton.Click += this.HandleEvent;
            this.Engine.UiControl.LoginWindow.CancleButton.Click += this.HandleEvent;
            this.Engine.UiControl.MainWindow.CloseButton.Click += this.HandleEvent;
        }
    }
}