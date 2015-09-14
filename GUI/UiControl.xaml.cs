using System;
using System.Collections.Generic;
using System.Windows;
using TerminologyLauncher.GUI.ToolkitWindows;
using TerminologyLauncher.GUI.ToolkitWindows.SingleLineInput;
using TerminologyLauncher.GUI.ToolkitWindows.SingleSelect;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for UiControl.xaml
    /// </summary>
    public partial class UiControl
    {
        public LoginWindow LoginWindow { get; set; }
        public MainWindow MajorWindow { get; set; }
        public UiControl()
        {
            this.MajorWindow = new MainWindow();
            this.MainWindow = this.MajorWindow;
            this.LoginWindow = new LoginWindow();
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
                this.MajorWindow.Dispatcher.Invoke(() => { this.MainWindow.Show(); });
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
                this.MajorWindow.Dispatcher.Invoke(() => { this.MainWindow.Hide(); });

            }
            catch (Exception ex)
            {
                Logging.Logger.GetLogger().ErrorFormat("Can not hide main window right now! Cause:{0}", ex.Message);
            }
        }

        public WindowResult StartSingleLineInput(String title, String fieldName)
        {
            WindowResult result = null;
            this.Dispatcher.Invoke(() =>
            {
                result = new SingleLineInputWindow(title, fieldName).ReceiveUserInput();
            });
            return result;
        }

        public WindowResult StartSingleSelect(String title, String fieldName, IEnumerable<String> selectItems)
        {
            WindowResult result = null;
            this.Dispatcher.Invoke(() =>
            {
                result = new SingleSelectWindow(title, fieldName, selectItems).ReceiveUserSelect();
            });
            return result;
        }

        public void StartPopupWindow(Window owner, String title, String content)
        {
            this.Dispatcher.Invoke(() =>
            {
                new PopupWindow(owner, title, content).ShowDialog();
            });
        }


        private void App_OnStartup(object sender, StartupEventArgs e)
        {

        }

        private void UiControl_OnExit(object sender, ExitEventArgs e)
        {
            this.MainWindow.Close();
            this.LoginWindow.Close();
        }
    }
}
