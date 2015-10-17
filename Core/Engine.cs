using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TerminologyLauncher.Auth;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Core.Handlers;
using TerminologyLauncher.Core.Handlers.LoginHandlers;
using TerminologyLauncher.Core.Handlers.MainHandlers;
using TerminologyLauncher.Core.Handlers.SystemHandlers;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Entities.FileRepository;
using TerminologyLauncher.FileRepositorySystem;
using TerminologyLauncher.GUI;
using TerminologyLauncher.I18n;
using TerminologyLauncher.InstanceManagerSystem;
using TerminologyLauncher.JreManagerSystem;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Updater;

namespace TerminologyLauncher.Core
{
    public sealed class Engine
    {
        #region Instance

        private static Engine Instance;

        public static Engine GetEngine()
        {
            return Instance ?? (Instance = new Engine());
        }

        #endregion

        public String CoreVersion
        {
            get { return "A2"; }
        }

        public String BuildVersion
        {
            get { return "1329"; }
        }

        public Config CoreConfig { get; set; }
        public TranslationProvider Translation { get; set; }
        public UiControl UiControl { get; set; }
        public AuthServer AuthServer { get; set; }
        public FileRepository FileRepo { get; set; }
        public InstanceManager InstanceManager { get; set; }
        public UpdateManager UpdateManager { get; set; }
        public Dictionary<String, HandlerBase> Handlers { get; set; }
        public JreManager JreManager { get; set; }
        public Process GameProcess { get; set; }
        public Engine()
        {
            Logger.GetLogger().InfoFormat("Engine {0} Initializing...", this.CoreVersion + this.BuildVersion);
            this.CoreConfig = new Config(new FileInfo("Configs/CoreConfig.json"));
            this.Translation = TranslationProvider.TranslationProviderInstance;
            this.UiControl = new UiControl(this.CoreConfig.GetConfigString("guiConfig"));
            this.AuthServer = new AuthServer(this.CoreConfig.GetConfigString("authConfig"));

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
            Engine.GetEngine().UiControl.Shutdown();
            Logger.GetLogger().Info("UiControl shutdown.");
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
            this.Handlers.Add("CONFIG", new ConfigHandler(this));
        }

        public void PostInitializeComponents()
        {
            Logger.GetLogger().Info("Engine extra component initializing...");
            this.FileRepo = new FileRepository(this.CoreConfig.GetConfigString("fileRepositoryConfig"));
            this.JreManager = new JreManager(this.CoreConfig.GetConfigString("jreManagerConfig"));
            this.InstanceManager = new InstanceManager(this.CoreConfig.GetConfigString("instanceManagerConfig"), this.FileRepo, this.JreManager);
            this.UpdateManager = new UpdateManager(this.CoreConfig.GetConfigString("updateManagerConfig"), this.CoreVersion + this.BuildVersion);
            Logger.GetLogger().Info("Engine extra component initialized...");
        }
    }
}
