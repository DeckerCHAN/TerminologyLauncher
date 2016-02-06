using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using TerminologyLauncher.Configs;
using TerminologyLauncher.GUI.Animations;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow;
using TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow.ConfigObjects;
using TerminologyLauncher.GUI.ToolkitWindows.NotifyWindow;
using TerminologyLauncher.GUI.ToolkitWindows.ProgressWindow;
using TerminologyLauncher.GUI.ToolkitWindows.SingleSelect;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for UiControl.xaml
    /// </summary>
    public partial class UiControl : IPopup
    {
        public new MainWindow MainWindow { get; set; }
        public LoginWindow LoginWindow { get; set; }
        public ConsoleWindow ConsoleWindow { get; set; }
        public Config Config { get; set; }
        public UiControl(String configPath)
        {
            this.Config = new Config(new FileInfo(configPath));
            this.MainWindow = new MainWindow(this.Config);
            this.LoginWindow = new LoginWindow(this.Config);
            this.ConsoleWindow = new ConsoleWindow(this.Config);
        }

        public Storyboard FadeInStoryboard
        {
            get { return Fade.CreateFadeInStoryboard(TimeSpan.FromMilliseconds(500)); }
        }

        public Storyboard FadeOutStoryboard
        {
            get
            {
                return Fade.CreateFadeOutStoryboard(TimeSpan.FromMilliseconds(500));
            }
        }

        public void ShowLoginWindow()
        {
            try
            {
                this.LoginWindow.Dispatcher.Invoke(() =>
                {
                    Fade.FadeInShowWindow(this.LoginWindow,TimeSpan.FromMilliseconds(300));
                });

            }
            catch (Exception ex)
            {
                Logging.Logger.GetLogger().FatalFormat("Cannot show login window right now! Cause:{0}", ex);
                this.Shutdown();
            }
        }

        public void HideLoginWindow()
        {
            try
            {
                this.LoginWindow.Dispatcher.Invoke(() =>
                {
                    Fade.FadeOutHideWindow(this.LoginWindow, TimeSpan.FromMilliseconds(300));
                });
            }
            catch (Exception ex)
            {
                Logging.Logger.GetLogger().FatalFormat("Cannot hide login window right now! Cause:{0}", ex);
                this.Shutdown();
            }
        }

        public void ShowMainWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(() =>
                {
                    Fade.FadeInShowWindow(this.MainWindow, TimeSpan.FromMilliseconds(300));
                });
            }
            catch (Exception ex)
            {

                Logging.Logger.GetLogger().FatalFormat("Cannot show main window right now! Cause:{0}", ex);
                this.Shutdown();
            }

        }
        public void HideMainWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(() =>
                {
                    Fade.FadeOutHideWindow(this.MainWindow, TimeSpan.FromMilliseconds(300));
                });

            }
            catch (Exception ex)
            {
                Logging.Logger.GetLogger().FatalFormat("Cannot hide main window right now! Cause:{0}", ex.Message);
                this.Shutdown();
            }
        }

        public void ShowConsoleWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(() => { Fade.FadeInShowWindow(this.ConsoleWindow, TimeSpan.FromMilliseconds(300)); });

            }
            catch (Exception ex)
            {
                Logging.Logger.GetLogger().FatalFormat("Cannot show console window right now! Cause:{0}", ex);
                this.Shutdown();
            }
        }

        public void HideConsoleWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(() => { Fade.FadeOutHideWindow(this.ConsoleWindow, TimeSpan.FromMilliseconds(300)); });

            }
            catch (Exception ex)
            {
                Logging.Logger.GetLogger().FatalFormat("Cannot hide console window right now! Cause:{0}", ex);
                this.Shutdown();
            }
        }

        public Boolean? StartConfigWindow(IEnumerable<TextInputConfigObject> textInputConfigs, IEnumerable<ItemSelectConfigObject> itemSelectConfigs, IEnumerable<RangeRestrictedSelectConfigObject> rangeRestrictedSelectConfigs)
        {
            Boolean? result = false;
            this.Dispatcher.Invoke(() =>
            {
                result = new ConfigWindow(textInputConfigs, itemSelectConfigs, rangeRestrictedSelectConfigs).ShowDialog();
            });
            return result;
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {

        }

        private void UiControl_OnExit(object sender, ExitEventArgs e)
        {
            this.MainWindow.Close();
            this.LoginWindow.Close();
        }

        public void PopupNotifyDialog(string title, string content)
        {
            this.Dispatcher.Invoke(() =>
            {
                new NotifyWindow(null, title, content).ShowDialog();
            });
        }

        public bool? PopupConfirmDialog(string title, string content)
        {
            throw new NotImplementedException();
        }

        public bool? PopupSingleSelectDialog(string title, string fieldName, IEnumerable<string> options, FieldReference<string> selection)
        {
            Boolean? result = false;
            this.Dispatcher.Invoke(() =>
            {
                var selectWindow = new SingleSelectWindow(null, title, fieldName, options, selection);
                result = selectWindow.ShowDialog();
            });
            return result;
        }

        public bool? PopupSingleLineInputDialog(string title, string fieldName, FieldReference<string> content)
        {
            throw new NotImplementedException();
        }

        public ProgressWindow BeginPopupProgressWindow(Progress progress)
        {
            ProgressWindow progressWindow = null;
            this.Dispatcher.InvokeAsync(() =>
            {
                progressWindow = new ProgressWindow(null, progress);
                progressWindow.Show();
            });
            return progressWindow;
        }
    }
}
