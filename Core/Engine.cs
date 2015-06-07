using System;
using TerminologyLauncher.GUI;

namespace TerminologyLauncher.Core
{
    public class Engine
    {
        #region Instance

        public static Engine Instance;

        public static Engine GetEngine()
        {
            return Instance ?? (Instance = new Engine());
        }

        #endregion
        public UiControl UiControl;

        public Engine()
        {
            Logging.Logger.GetLogger().Info("Engine Initializing...");
            this.UiControl = new UiControl();
            Logging.Logger.GetLogger().Info("Engine Initialized!");
        }
        public void Run()
        {
            Logging.Logger.GetLogger().Info("Engine running...");
            this.UiControl.ShowLoginWindow();
            //Logging.Logger.GetLogger().Info("Start GUI");
            this.UiControl.Run();

        }
    }
}
