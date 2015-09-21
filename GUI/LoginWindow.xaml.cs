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
    public partial class LoginWindow : INotifyPropertyChanged
    {
        private bool IsPerservePasswordValue;

        public delegate void LogingHandler(Object serder, EventArgs e);

        public event LogingHandler Logining;
        public LoginWindow()
        {
            this.InitializeComponent();
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
            this.Dispatcher.InvokeAsync(() =>
            {
                this.UsernameBox.IsEnabled = isEnable;
                this.PasswordBox.IsEnabled = isEnable;
                this.LoginModeComboBox.IsEnabled = isEnable;
                this.IsPerservePasswordCheckBox.IsEnabled = isEnable;
                this.CancleButton.IsEnabled = isEnable;
                this.LoginButton.IsEnabled = isEnable;
            });
        }

        public void CrossThreadClose()
        {
            this.Dispatcher.Invoke(this.Close);
        }

        public Boolean IsPerservePassword
        {
            get { return this.IsPerservePasswordValue; }
            set
            {
                this.IsPerservePasswordValue = value;
                this.OnPropertyChanged();
            }
        }

        public LoginEntity GetLogin()
        {
            LoginEntity login = null;
            this.Dispatcher.Invoke(() =>
            {
                login = new LoginEntity()
                {
                    UserName = this.UsernameBox.Text,
                    Password = this.PasswordBox.Password,
                    LoginType = (LoginType)this.LoginModeComboBox.SelectedIndex,
                    PerserveLogin = this.IsPerservePassword
                };
            });
            if (login == null)
            {
                throw new Exception("Can not get valid login entity");
            }
            return login;

        }

        public void SetLogin(LoginEntity login)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.UsernameBox.Text = login.UserName;
                this.PasswordBox.Password = login.Password;
                this.LoginModeComboBox.SelectedIndex = (int)login.LoginType;
                this.IsPerservePassword = login.PerserveLogin;
            });

        }

        public void LoginResult(LoginResultType result)
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (result)
                {
                    case LoginResultType.Success:
                        {
                            this.Hide();
                            break;
                        }
                    case LoginResultType.IncompleteOfArguments:
                        {
                            new PopupWindow(this, "失败", "参数不完整").ShowDialog();
                            break;
                        }
                    case LoginResultType.WrongPassword:
                        {
                            new PopupWindow(this, "失败", "密码错误").ShowDialog();
                            break;
                        }
                    case LoginResultType.UserNotExists:
                        {
                            new PopupWindow(this, "失败", "用户不存在").ShowDialog();
                            break;
                        }
                    case LoginResultType.NetworkTimedOut:
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
                this.EnableAllInputs(true);
            });

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

        protected virtual void OnLogining(object serder)
        {
            var handler = this.Logining;
            if (handler != null) handler(serder, EventArgs.Empty);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.OnLogining(this);
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.OnLogining(this);
        }
    }
}
