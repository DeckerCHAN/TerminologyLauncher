using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TerminologyLauncher.GUI.Annotations;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
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

        public void DisableAllInputs()
        {
            this.UsernameBox.IsEnabled = false;
            this.PasswordBox.IsEnabled = false;
            this.LoginModeComboBox.IsEnabled = false;
            this.IsPerservePasswordCheckBox.IsEnabled = false;
        }

        public void EnableAllInputs()
        {
            this.UsernameBox.IsEnabled = true;
            this.PasswordBox.IsEnabled = true;
            this.LoginModeComboBox.IsEnabled = true;
            this.IsPerservePasswordCheckBox.IsEnabled = true;
        }

        private void LoginMode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combox = sender as ComboBox;
            var selected = combox.SelectedIndex;
            this.AccountTypeTitle.Text = selected == 1 ? "Majong账户:" : "用户名:";

            if (selected == 0)
            {
                this.AccountPasswordTitle.Visibility = Visibility.Hidden;
                this.PasswordBox.Visibility = Visibility.Hidden;

            }
            else
            {
                this.AccountPasswordTitle.Visibility = Visibility.Visible;
                this.PasswordBox.Visibility = Visibility.Visible;
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
