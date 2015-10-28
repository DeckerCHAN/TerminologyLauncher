using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using TerminologyLauncher.Configs;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.GUI.ToolkitWindows;
using TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow;
using TerminologyLauncher.GUI.ToolkitWindows.ConfigWindow.ConfigObjects;
using TerminologyLauncher.GUI.ToolkitWindows.PopupWindow;
using TerminologyLauncher.GUI.ToolkitWindows.SingleLineInput;
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

        public void ShowLoginWindow()
        {
            try
            {
                this.LoginWindow.Dispatcher.Invoke(() => { this.LoginWindow.Show(); });

            }
            catch (Exception ex)
            {
                Logging.Logger.GetLogger().ErrorFormat("Can not show login window right now! Cause:{0}", ex.Message);
            }
        }

        public void HideLoginWindow()
        {
            try
            {
                this.LoginWindow.Dispatcher.Invoke(() => { this.LoginWindow.Hide(); });
            }
            catch (Exception ex)
            {
                Logging.Logger.GetLogger().ErrorFormat("Can not hide login window right now! Cause:{0}", ex.Message);
            }
        }

        public void ShowMainWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(() => { this.MainWindow.Show(); });
            }
            catch (Exception ex)
            {

                Logging.Logger.GetLogger().ErrorFormat("Can not show main window right now! Cause:{0}", ex.Message);
            }

        }
        public void HideMainWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(() => { this.MainWindow.Hide(); });

            }
            catch (Exception ex)
            {
                Logging.Logger.GetLogger().ErrorFormat("Can not hide main window right now! Cause:{0}", ex.Message);
            }
        }

        public void ShowConsoleWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(() => { this.ConsoleWindow.Show(); });

            }
            catch (Exception ex)
            {
                Logging.Logger.GetLogger().ErrorFormat("Can not show console window right now! Cause:{0}", ex.Message);
            }
        }

        public void HideConsoleWindow()
        {
            try
            {
                this.MainWindow.Dispatcher.Invoke(() => { this.ConsoleWindow.Hide(); });

            }
            catch (Exception ex)
            {
                Logging.Logger.GetLogger().ErrorFormat("Can not hide console window right now! Cause:{0}", ex.Message);
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

        public bool? PopupConfirmDialog(string title, string content, bool? decision)
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
            throw new NotImplementedException();
        }
    }
}
