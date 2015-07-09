using System.Windows;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for UiControl.xaml
    /// </summary>
    public partial class UiControl
    {
        public LoginWindow LoginWindow { get; set; }
        public UiControl()
        {
            this.MainWindow = new MainWindow();
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
            this.MainWindow.Dispatcher.Invoke(() => { this.MainWindow.Show(); });
        }
        public void HideMainWindow()
        {
            this.MainWindow.Dispatcher.Invoke(() => { this.MainWindow.Hide(); });
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
