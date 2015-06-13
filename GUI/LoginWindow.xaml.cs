using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Logging.Logger.GetLogger().Info("Start drag!");
            this.DragMove();
        }

        private void ToggleButton_OnCheckedChanged(object sender, RoutedEventArgs e)
        {
            var checkbox = (CheckBox)sender;
            var isChecked = (checkbox).IsChecked;
            if (isChecked == null) return;
            this.PasswordBox.Password = String.Empty;
            if ((bool)isChecked)
            {
                this.PasswordBox.IsEnabled = false;
            }
            else
            {
                this.PasswordBox.IsEnabled = true;
            }
        }

        private void LoginButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.UsernameBox.IsEnabled = false;
            this.PasswordBox.IsEnabled = false;
        }
    }
}
