using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TerminologyLauncher.Entities.Account;
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

        public void EnableAllInputs(Boolean isEnable)
        {
            this.UsernameBox.IsEnabled = isEnable;
            this.PasswordBox.IsEnabled = isEnable;
            this.LoginModeComboBox.IsEnabled = isEnable;
            this.IsPerservePasswordCheckBox.IsEnabled = isEnable;
        }

        public Login GetLogin()
        {
            return new Login()
            {
                UserName = this.UsernameBox.Text,
                Password = this.PasswordBox.Password,
                LoginMode = (LoginMode)this.LoginModeComboBox.SelectedIndex
            };
        }

        public void SetLogin(Login login)
        {
            this.UsernameBox.Text = login.UserName;
            this.PasswordBox.Password = login.Password;
            this.LoginModeComboBox.SelectedIndex = (int)login.LoginMode;
        }

        public void LoginResult(LoginResult result)
        {
            this.EnableAllInputs(true);
            switch (result)
            {
                case Entities.Account.LoginResult.Success:
                    {
                        this.Hide();
                        break;
                    }
                case Entities.Account.LoginResult.InsufficiencyOfArguments:
                    {
                        new PopupWindow(this, "失败", "参数不完整").ShowDialog();
                        break;
                    }
                case Entities.Account.LoginResult.WrongPassword:
                    {
                        new PopupWindow(this, "失败", "密码错误").ShowDialog();
                        break;
                    }
                case Entities.Account.LoginResult.UserNotExists:
                    {
                        new PopupWindow(this, "失败", "用户不存在").ShowDialog();
                        break;
                    }
                case Entities.Account.LoginResult.NetworkTimedOut:
                    {
                        new PopupWindow(this, "失败", "网络超时").ShowDialog();
                        break;
                    }
                default:
                    {
                        new PopupWindow(this, "失败", "未知错误").ShowDialog();
                        break;
                    }
            }
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
