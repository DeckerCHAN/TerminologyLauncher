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
            this.UiControl = new UiControl();
            this.UiControl.MainWindow.Dispatcher.Invoke(new Action(() => { this.UiControl.MainWindow.Show(); }));
        }
        public void Run()
        {
            this.UiControl.Run();
        }
    }
}
