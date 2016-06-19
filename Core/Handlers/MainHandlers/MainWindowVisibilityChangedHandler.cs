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
using TerminologyLauncher.Updater;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.Core.Handlers.MainHandlers
{
    public class MainWindowVisibilityChangedHandler : HandlerBase
    {
        private bool FirstStart;

        public MainWindowVisibilityChangedHandler(Engine engine)
            : base(engine)
        {
            this.FirstStart = true;
            this.Engine.UiControl.MainWindow.IsVisibleChanged += this.HandleEvent;
        }

        public void HandleEvent(object sender, DependencyPropertyChangedEventArgs e)
        {

            var window = sender as Window;
            TerminologyLogger.GetLogger().InfoFormat("MainWindow window is going to {0}!", window.Visibility);
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
                            this.Engine.UiControl.MainWindow.CoreVersion =
                                $"{this.Engine.CoreVersion} (build{this.Engine.BuildVersion})";

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
                                    try
                                    {
                                        var result = this.Engine.InstanceManager.CheckAllInstanceCouldUpdate();
                                        if (!string.IsNullOrEmpty(result))
                                        {
                                            this.Engine.UiControl.MainWindow.PopupNotifyDialog(TranslationManager.GetManager.Localize("InstanceUpdateNotifyTitle", "Check Update"), result);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        TerminologyLogger.GetLogger().ErrorFormat("Cause by a error, can not check instance update right now! Detail:{0}", ex.Message);
                                        throw;
                                    }
                                });

                            }
                            Task.Run(() =>
                            {
                                try
                                {
                                    var message = string.Empty;
                                    var updateInfo = this.Engine.UpdateManager.GetupdateInfo();

                                    switch (updateInfo.UpdateType)
                                    {
                                        case UpdateType.Higher:
                                            message =
                                                $"There is higher version({updateInfo.LatestVersion.CoreVersion}-{updateInfo.LatestVersion.BuildNumber}) available";
                                            break;
                                        case UpdateType.Lower:
                                            message =
                                                $"You are using the test version or pre-release version({updateInfo.LatestVersion.CoreVersion}-{updateInfo.LatestVersion.BuildNumber}). DO NOT DISTRIBUTE THIS VERSION!";
                                            break;
                                        default:
                                            break;
                                    }
                                    if (!string.IsNullOrEmpty(message))
                                    {
                                        this.Engine.UiControl.MainWindow.PopupNotifyDialog(TranslationManager.GetManager.Localize("LanucherUpdateNotifyTitle", "Lanucher Update"), message);

                                    }

                                }
                                catch (Exception ex)
                                {
                                    TerminologyLogger.GetLogger().ErrorFormat("Cause by a error, can not check update right now! Detail:{0}", ex.Message);
                                    throw;
                                }


                            });

                        }
                        break;
                    }
                default:
                    {
                        TerminologyLogger.GetLogger().Error($"HandlerBase could not handle {window.Visibility} status.");
                        break;
                    }
            }


        }
        public override void HandleEvent(object sender, EventArgs e)
        {
            throw new NotSupportedException();
        }

        private bool CheckJavaPath()
        {
            //Check config 
            if (this.Engine.JreManager.JavaRuntime != null)
            {
                return true;
            }


            var javaRuntimeEntitiesKP = new Dictionary<string, JavaRuntimeEntity>();
            foreach (var availableJre in this.Engine.JreManager.AvailableJavaRuntimes)
            {

                javaRuntimeEntitiesKP.Add(availableJre.Key, availableJre.Value);

            }
            if (javaRuntimeEntitiesKP.Keys.Count != 0)
            {
                var field = new FieldReference<string>(javaRuntimeEntitiesKP.Keys.First());
                var result = this.Engine.UiControl.PopupSingleSelectDialog(TranslationManager.GetManager.Localize("JavaSelectWindowTitle", "Select a Java exe"), TranslationManager.GetManager.Localize("JavaSelectField", "Available Java exe:"), javaRuntimeEntitiesKP.Keys, field);
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
                TerminologyLogger.GetLogger().Warn("Java path is empty. Try to receive from user..");

                var field = new FieldReference<string>(string.Empty);
                var result = this.Engine.UiControl.PopupSingleLineInputDialog(TranslationManager.GetManager.Localize("JavaInputWindowTitle", "Input a Java exe"), TranslationManager.GetManager.Localize("JavaInputField", "Java(not javaw) exe path:"), field);

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
                            TerminologyLogger.GetLogger()
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
