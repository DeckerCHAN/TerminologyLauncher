using TerminologyLauncher.Auth;
using TerminologyLauncher.Core.Handlers;
using TerminologyLauncher.Core.Handlers.LoginHandlers;
using TerminologyLauncher.Core.Handlers.MainHandlers;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Entities.FileRepository;
using TerminologyLauncher.FileRepository;
using TerminologyLauncher.GUI;
using TerminologyLauncher.Logging;

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
        public UiControl UiControl { get; set; }
        public AuthServer AuthServer { get; set; }
        public FileRepository.FileRepository FileRepo { get; set; }
        public InstanceManager.InstanceManager InstanceManager { get; set; }
        public PlayerEntity CurrentPlayer { get; set; }
        public Engine()
        {
            Logger.GetLogger().Info("Engine Initializing...");
            this.UiControl = new UiControl();
            this.AuthServer = new AuthServer();
            Logger.GetLogger().Info("Engine Initialized!");
        }
        public void Run()
        {
            this.RegisterEvents();
            Logger.GetLogger().Info("Engine running...");
            this.UiControl.ShowLoginWindow();
            Logger.GetLogger().Info("Starting GUI...");
            this.UiControl.Run();
            Logger.GetLogger().Info("Exit running.");
        }

        public void Exit()
        {
            Logger.GetLogger().Info("Engine shutting down...");
        }

        public void RegisterEvents()
        {
            Logger.GetLogger().Debug("Engine Register events.");
            //TODO:Using IHandler interface, let handlers register their events duding ctor.
            this.UiControl.LoginWindow.CloseButton.Click += new CloseHandler().HandleEvent;
            this.UiControl.LoginWindow.CancleButton.Click += new CloseHandler().HandleEvent;
            this.UiControl.LoginWindow.LoginButton.Click += new LoginHandler().HandleEvent;
            this.UiControl.LoginWindow.IsVisibleChanged += new LoginWindowVisibilityChangedHandler().HandleEvent;
            this.UiControl.MainWindow.IsVisibleChanged += new MainWindowVisibilityChangedHandler().HandleEvent;
        }

        public void InitializeComponents()
        {
            Logger.GetLogger().Info("Engine extra component initializing...");
            this.FileRepo = new FileRepository.FileRepository();
            this.InstanceManager = new InstanceManager.InstanceManager();
            Logger.GetLogger().Info("Engine extra component initialized...");
        }
    }
}
