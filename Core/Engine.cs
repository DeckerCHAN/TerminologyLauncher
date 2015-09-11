using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TerminologyLauncher.Auth;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Core.Handlers;
using TerminologyLauncher.Core.Handlers.LoginHandlers;
using TerminologyLauncher.Core.Handlers.MainHandlers;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Entities.FileRepository;
using TerminologyLauncher.FileRepositorySystem;
using TerminologyLauncher.GUI;
using TerminologyLauncher.InstanceManagerSystem;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Updater;

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

        public String CoreVersion
        {
            get { return "A1"; }
        }

        public Config CoreConfig { get; set; }
        public UiControl UiControl { get; set; }
        public AuthServer AuthServer { get; set; }
        public FileRepository FileRepo { get; set; }
        public InstanceManager InstanceManager { get; set; }
        public UpdateManager UpdateManager { get; set; }
        public Dictionary<String, HandlerBase> Handlers { get; set; }
        public Process GameProcess { get; set; }
        public Engine()
        {
            Logger.GetLogger().Info("Engine Initializing...");
            this.CoreConfig = new Config(new FileInfo("Configs/CoreConfig.json"));
            this.UiControl = new UiControl();
            this.AuthServer = new AuthServer(this.CoreConfig.GetConfig("authConfig"));

            this.Handlers = new Dictionary<string, HandlerBase>();
            Logger.GetLogger().Info("Engine Initialized!");
        }
        public void Run()
        {
            this.RegisterHandlers();
            Logger.GetLogger().Info("Engine running...");
            Logger.GetLogger().Info("Starting GUI...");
            this.UiControl.ShowLoginWindow();
            this.UiControl.Run();
            Logger.GetLogger().Info("Exit running.");
        }

        public void Exit()
        {
            Logger.GetLogger().Info("Engine shutting down...");
        }

        public void RegisterHandlers()
        {
            Logger.GetLogger().Debug("Engine Register events.");
            //DONE:Using IHandler interface, let handlers register their events duding ctor.
            this.Handlers.Add("WINDOWS_CLOSE", new CloseHandler(this));
            this.Handlers.Add("LOGIN", new LoginHandlerBase(this));
            this.Handlers.Add("LOGIN_WINDOW_VISIBILITY_CHANGED", new LoginWindowVisibilityChangedHandler(this));
            this.Handlers.Add("MAIN_WINDOW_VISIBILITY_CHANGED", new MainWindowVisibilityChangedHandler(this));
            this.Handlers.Add("ADD_NEW_INSTANCE", new AddInstanceHandler(this));
            this.Handlers.Add("REMOVE_AN_INSTANCE", new RemoveInstanceHandler(this));
            this.Handlers.Add("LAUNCH_AN_INSTANCE", new LaunchInstanceHandler(this));
            this.Handlers.Add("UPDATE_AN_INSTANCE", new UpdateInstanceHandler(this));
            this.Handlers.Add("UPDATE_APPLICATION", new UpdateApplicationHandler(this));

        }

        public void PostInitializeComponents()
        {
            Logger.GetLogger().Info("Engine extra component initializing...");
            this.FileRepo = new FileRepository(this.CoreConfig.GetConfig("fileRepositoryConfig"));
            this.InstanceManager = new InstanceManager(this.CoreConfig.GetConfig("instanceManagerConfig"), this.FileRepo);
            this.UpdateManager = new UpdateManager(this.CoreConfig.GetConfig("updateManagerConfig"), this.CoreVersion);
            Logger.GetLogger().Info("Engine extra component initialized...");
        }
    }
}
