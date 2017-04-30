using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using TerminologyLauncher.Auth;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Core.Handlers;
using TerminologyLauncher.Core.Handlers.LoginHandlers;
using TerminologyLauncher.Core.Handlers.MainHandlers;
using TerminologyLauncher.Core.Handlers.SystemHandlers;
using TerminologyLauncher.FileRepositorySystem;
using TerminologyLauncher.GUI;
using TerminologyLauncher.InstanceManagerSystem;
using TerminologyLauncher.JreManagerSystem;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Updater;
using TerminologyLauncher.Utils;

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

        public string CoreVersion => ResourceUtils.ReadEmbedFileAsString("TerminologyLauncher.Core.Version.txt");

        public int BuildVersion
            => Convert.ToInt32(ResourceUtils.ReadEmbedFileAsString("TerminologyLauncher.Core.Build.txt"));

        public Config CoreConfig { get; set; }
        public UiControl UiControl { get; set; }
        public AuthServer AuthServer { get; set; }
        public FileRepository FileRepo { get; set; }
        public InstanceManager InstanceManager { get; set; }
        public InstanceCreator InstanceCreator { get; set; }
        public UpdateManager UpdateManager { get; set; }
        public Dictionary<string, HandlerBase> Handlers { get; set; }
        public JreManager JreManager { get; set; }
        public Process GameProcess { get; set; }
        public Dispatcher EngineDispatcher { get; private set; }

        public Engine()
        {
            TerminologyLogger.GetLogger().InfoFormat($"Os version:{Environment.NewLine + MachineUtils.GetOsVersion()}");
            TerminologyLogger.GetLogger()
                .InfoFormat($"Dot net versions:{Environment.NewLine + MachineUtils.GetNetVersionFromRegistry()}");
            TerminologyLogger.GetLogger().InfoFormat($"Engine {this.CoreVersion + this.BuildVersion} Initializing...");
            this.EngineDispatcher = Dispatcher.CurrentDispatcher;
            this.CoreConfig = new Config(new FileInfo("Configs/CoreConfig.json"));
            this.UiControl = new UiControl(this.CoreConfig.GetConfigString("guiConfig"));
            this.AuthServer = new AuthServer(this.CoreConfig.GetConfigString("authConfig"));

            this.Handlers = new Dictionary<string, HandlerBase>();
            TerminologyLogger.GetLogger().Info("Engine Initialized!");
        }

        public void Run()
        {
            this.RegisterHandlers();
            TerminologyLogger.GetLogger().Info("Engine running...");
            TerminologyLogger.GetLogger().Info("Starting GUI...");
            this.UiControl.ShowLoginWindow();
            this.UiControl.Run();
            TerminologyLogger.GetLogger().Info("Exit running.");
        }

        public void Exit()
        {
            this.EngineDispatcher.Invoke(() =>
            {
                TerminologyLogger.GetLogger().Info("Engine shutting down...");
                this.UiControl.Shutdown();
                TerminologyLogger.GetLogger().Info("UiControl shutdown.");
            });
        }

        public void RegisterHandlers()
        {
            //TODO:In Future, this process may managed by an individual module. Also, may load external handler by reflection.
            TerminologyLogger.GetLogger().Debug("Engine Register events.");

            //Get all types who impl/extend Hadler Base
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(HandlerBase).IsAssignableFrom(p)))
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }
                var handlerInstance = (HandlerBase) Activator.CreateInstance(type, this);
                if (this.Handlers.ContainsKey(handlerInstance.Name))
                {
                    this.Handlers.Remove(handlerInstance.Name);
                    TerminologyLogger.GetLogger().InfoFormat($"{handlerInstance.Name} already exists. Overloaded by new instance.");
                }
                this.Handlers.Add(handlerInstance.Name, handlerInstance);
                TerminologyLogger.GetLogger().InfoFormat($"Successfuly loadded Handler: {handlerInstance.Name}");
            }
        }

        public void PostInitializeComponents()
        {
            try
            {
                TerminologyLogger.GetLogger().Info("Engine extra component initializing...");
                this.FileRepo = new FileRepository(this.CoreConfig.GetConfigString("fileRepositoryConfig"));
                this.JreManager = new JreManager(this.CoreConfig.GetConfigString("jreManagerConfig"));
                this.InstanceManager = new InstanceManager(this.CoreConfig.GetConfigString("instanceManagerConfig"),
                    this.FileRepo, this.JreManager);

                this.UpdateManager = new UpdateManager(this.CoreConfig.GetConfigString("updateManagerConfig"),
                    this.CoreVersion, this.BuildVersion);
                TerminologyLogger.GetLogger().Info("Engine extra component initialized...");
            }
            catch (Exception ex)
            {
                TerminologyLogger.GetLogger()
                    .FatalFormat(
                        $"Enging encountered an fatal during post initialize. This exception caused by {ex.Message}. Launcher shuting down.\n Detail:{ex.ToString()}");
                throw;
            }
        }
    }
}