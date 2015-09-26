using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.System.Java;
using TerminologyLauncher.GUI;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.GUI.ToolkitWindows;
using TerminologyLauncher.I18n;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.Core.Handlers.MainHandlers
{
    public class MainWindowVisibilityChangedHandler : HandlerBase
    {
        private Boolean FirstStart;

        public MainWindowVisibilityChangedHandler(Engine engine)
            : base(engine)
        {
            this.FirstStart = true;
            this.Engine.UiControl.MainWindow.IsVisibleChanged += this.HandleEvent;
        }

        public void HandleEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var window = sender as Window;
            Logger.GetLogger().InfoFormat("Main window is going to {0}!", window.Visibility);

            switch (window.Visibility)
            {
                case Visibility.Hidden:
                    {

                        break;
                    }
                case Visibility.Visible:
                    {
                        if (this.FirstStart)
                        {
                            this.FirstStart = false;

                            if (!this.CheckJavaPath())
                            {
                                this.Engine.Exit();
                                return;
                            }

                            this.Engine.UiControl.MainWindow.InstanceList =
                                  new ObservableCollection<InstanceEntity>(this.Engine.InstanceManager.InstancesWithLocalImageSource);
                            if (this.Engine.UiControl.MainWindow.InstanceList.Count == 0)
                            {
                                var addHandler = this.Engine.Handlers["ADD_NEW_INSTANCE"] as AddInstanceHandler;
                                if (addHandler != null) addHandler.HandleEvent(new object(), new EventArgs());
                            }



                            this.Engine.UiControl.MainWindow.CoreVersion = this.Engine.CoreVersion;
                        }
                        break;
                    }
                default:
                    {
                        Logger.GetLogger().Error(String.Format("HandlerBase could not handle {0} status.", window.Visibility));
                        break;
                    }
            }
            return;
        }
        public override void HandleEvent(Object sender, EventArgs e)
        {
            throw new NotSupportedException();
        }

        private Boolean CheckJavaPath()
        {
            //Check config 
            if (!String.IsNullOrEmpty(this.Engine.InstanceManager.Config.GetConfig("javaBinPath")))
            {
                try
                {
                    var javaBinFolder = new DirectoryInfo(this.Engine.InstanceManager.Config.GetConfig("javaBinPath"));
                    if (javaBinFolder.Exists && File.Exists(Path.Combine(javaBinFolder.FullName, "java.exe")) || File.Exists(Path.Combine(javaBinFolder.FullName, "javaw.exe")))
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    //ignore
                }

            }
            //Search java from default path
            var searchPaths = this.Engine.CoreConfig.GetConfigs("javaSearchPaths");

            var javaPaths = searchPaths.Where(Directory.Exists)
                .SelectMany(
                path => Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly),
                (path, folder) => Path.Combine(folder, "bin/java.exe")
                ).Where(File.Exists)
                .ToList();




            var javaRuntimeEntitiesKP = new Dictionary<String, JavaRuntimeEntity>();
            foreach (var javaPath in javaPaths)
            {
                try
                {
                    var runtimeEntity = new JavaRuntimeEntity()
                    {
                        JavaPath = javaPath,
                        JavaWPath = Path.Combine(Path.GetDirectoryName(javaPath), "javaw.exe"),
                        JavaDetails = JavaUtils.GetJavaDetails(javaPath)
                    };
                    javaRuntimeEntitiesKP.Add(String.Format("{0}:{1}", runtimeEntity.JavaDetails.JavaVersion, runtimeEntity.JavaDetails.JavaType), runtimeEntity);
                }
                catch (Exception)
                {
                    //Ignore
                }
            }
            if (javaRuntimeEntitiesKP.Keys.Count != 0)
            {
                var result = this.Engine.UiControl.StartSingleSelect(TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation.JavaSelectTranslation.JavaSelectWindowTitleTranslation, TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation.JavaSelectTranslation.JavaSelectFieldTranslation, javaRuntimeEntitiesKP.Keys);
                if (result.Type == WindowResultType.Canceled)
                {
                    return false;
                }
                else
                {
                    this.Engine.InstanceManager.Config.SetConfig("javaBinPath", Directory.GetParent(javaRuntimeEntitiesKP[result.Result.ToString()].JavaWPath).FullName);
                    return true;
                }
            }



            while (String.IsNullOrEmpty(this.Engine.InstanceManager.Config.GetConfig("javaBinPath")))
            {
                Logger.GetLogger().Warn("Java path is empty. Try to receive from user..");

                var result = this.Engine.UiControl.StartSingleLineInput("Request Java path", "Java Path");
                switch (result.Type)
                {
                    case WindowResultType.CommonFinished:
                        {
                            try
                            {
                                var javaExe = new FileInfo(result.Result.ToString());
                                if (javaExe.Exists && (javaExe.Name == "java.exe" || javaExe.Name == "javaw.exe"))
                                {
                                    this.Engine.InstanceManager.Config.SetConfig("javaBinPath", javaExe.DirectoryName);
                                    Logger.GetLogger().Info("Received java path from user. Pass.");
                                }
                            }
                            catch (Exception)
                            {

                                //ignore.
                            }
                            break;
                        }
                    case WindowResultType.Canceled:
                        return false;
                }
            }
            return true;
        }
    }
}
