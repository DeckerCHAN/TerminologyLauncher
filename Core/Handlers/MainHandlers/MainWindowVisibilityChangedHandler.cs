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



                            this.Engine.UiControl.MainWindow.CoreVersion = String.Format("{0} (build{1})", this.Engine.CoreVersion, this.Engine.BuildVersion);
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
            if (this.Engine.JreManager.JavaRuntime != null)
            {
                return true;
            }


            var javaRuntimeEntitiesKP = new Dictionary<String, JavaRuntimeEntity>();
            foreach (var availableJre in this.Engine.JreManager.AvailableJavaRuntimes)
            {

                javaRuntimeEntitiesKP.Add(availableJre.Key, availableJre.Value);

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
                    this.Engine.JreManager.JavaRuntime = javaRuntimeEntitiesKP[result.Result.ToString()];
                    return true;
                }
            }



            while (this.Engine.JreManager.JavaRuntime == null)
            {
                Logger.GetLogger().Warn("Java path is empty. Try to receive from user..");

                var result = this.Engine.UiControl.StartSingleLineInput("Request Java bin path", "Java bin folder");
                switch (result.Type)
                {
                    case WindowResultType.CommonFinished:
                        {
                            try
                            {
                                var javaBinFolder = new DirectoryInfo(result.Result.ToString());
                                var jre = new JavaRuntimeEntity
                                {
                                    JavaDetails =
                                        JavaUtils.GetJavaDetails(Path.Combine(javaBinFolder.FullName, "java.exe")),
                                    JavaPath = Path.Combine(javaBinFolder.FullName, "java.exe"),
                                    JavaWPath = Path.Combine(javaBinFolder.FullName, "javaw.exe")
                                };
                                if (JavaUtils.IsJavaRuntimeValid(jre))
                                {
                                    this.Engine.JreManager.JavaRuntime = jre;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            catch (Exception)
                            {
                                //ignore.
                                continue;
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
