using System;
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
        private void App_OnStartup(object sender, StartupEventArgs e)
        {

        }
    }
}
