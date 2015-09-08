using System;
using System.Windows;
using TerminologyLauncher.GUI.SingleLineInput;

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
            this.LoginWindow.Dispatcher.Invoke(() => { this.LoginWindow.Show(); });
        }

        public void HideLoginWindow()
        {
            this.LoginWindow.Dispatcher.Invoke(() => { this.LoginWindow.Hide(); });
        }

        public void ShowMainWindow()
        {
            this.MajorWindow.Dispatcher.Invoke(() => { this.MainWindow.Show(); });
        }
        public void HideMainWindow()
        {
            try
            {
                this.MajorWindow.Dispatcher.Invoke(() => { this.MainWindow.Hide(); });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public SingleLineInputResult StartSingleLineInput(String title, String field)
        {
            SingleLineInputResult result = null;
            this.Dispatcher.Invoke(() =>
            {
                result = new SingleLineInputWindow(title, field).ReceiveUserinput();
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
