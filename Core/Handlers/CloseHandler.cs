using System;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.Core.Handlers
{
    public class CloseHandler:IHandler
    {
        public void HandleEvent(object sender, EventArgs e)
        {
            Logger.GetLogger().Info("Handling close event.");
            Engine.GetEngine().UiControl.Shutdown();
            Logger.GetLogger().Info("UiControl shutdown.");
            Engine.GetEngine().Exit();
        }
    }
}
