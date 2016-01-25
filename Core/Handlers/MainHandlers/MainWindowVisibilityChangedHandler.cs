using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.System.Java;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.I18n;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.ProgressService;

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
            Logger.GetLogger().InfoFormat("MainWindow window is going to {0}!", window.Visibility);
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
                            this.Engine.UiControl.MainWindow.CoreVersion = String.Format("{0} (build{1})", this.Engine.CoreVersion, this.Engine.BuildVersion);

                            if (!this.CheckJavaPath())
                            {
                                this.Engine.Exit();
                                return;
                            }

                            this.Engine.UiControl.MainWindow.InstanceList =
                                  new ObservableCollection<InstanceEntity>(this.Engine.InstanceManager.InstancesWithLocalImageSource);

                            if (this.Engine.InstanceManager.Instances.Count == 0)
                            {
                                var addHandler = this.Engine.Handlers["ADD_NEW_INSTANCE"] as AddInstanceHandler;
                                if (addHandler != null) addHandler.HandleEvent(new object(), new EventArgs());
                            }
                            else
                            {
                                Task.Run(() =>
                                {
                                    var progress = new InternalNodeProgress("Check update");
                                    var result = this.Engine.InstanceManager.CheckAllInstanceCouldUpdate(progress);
                                    if (!String.IsNullOrEmpty(result))
                                    {
                                        this.Engine.UiControl.MainWindow.PopupNotifyDialog(I18n.TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation.InstanceUpdateTranslation.InstanceUpdateWindowTitleTranslation, result);
                                    }
                                });

                            }
                            Task.Run(() =>
                            {
                                try
                                {
                                    if (this.Engine.UpdateManager.IsNewVersionAvailable())
                                    {
                                        this.Engine.UiControl.MainWindow.PopupNotifyDialog(TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation.LanucherUpdateTranslation.LanucherUpdateWindowTitleTranslation, this.Engine.UpdateManager.GetUpdateInformationHumanReadable());
                                    }
                                }
                                catch (Exception)
                                {
                                    this.Engine.Exit();
                                }


                            });

                        }
                        break;
                    }
                default:
                    {
                        Logger.GetLogger().Error(String.Format("HandlerBase could not handle {0} status.", window.Visibility));
                        break;
                    }
            }


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
                var field = new FieldReference<String>(javaRuntimeEntitiesKP.Keys.First());
                var result = this.Engine.UiControl.PopupSingleSelectDialog(TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation.JavaSelectTranslation.JavaSelectWindowTitleTranslation, TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation.JavaSelectTranslation.JavaSelectFieldTranslation, javaRuntimeEntitiesKP.Keys, field);
                if (result == null || result.Value == false)
                {
                    return false;
                }
                else
                {
                    this.Engine.JreManager.JavaRuntime = javaRuntimeEntitiesKP[field.Value];
                    return true;
                }
            }



            while (this.Engine.JreManager.JavaRuntime == null)
            {
                Logger.GetLogger().Warn("Java path is empty. Try to receive from user..");

                var field = new FieldReference<String>(String.Empty);
                var result = this.Engine.UiControl.PopupSingleLineInputDialog(TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation.JavaSelectTranslation.JavaInputWindowTitleTranslation, TranslationProvider.TranslationProviderInstance.TranslationObject.HandlerTranslation.JavaSelectTranslation.JavaInputFieldTranslation, field);

                if (result == null || result.Value == false)
                {
                    {
                        try
                        {
                            var javaBinFolder = new DirectoryInfo(field.Value);
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
                        catch (Exception ex)
                        {
                            Logger.GetLogger()
                                .ErrorFormat("cannot resolve java exe path through user input. Caused by:{0}",
                                    ex.Message);

                            continue;
                        }
                        break;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
