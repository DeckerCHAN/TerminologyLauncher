using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.GUI.Properties;
using TerminologyLauncher.GUI.Toolkits;
using TerminologyLauncher.GUI.ToolkitWindows.NotifyWindow;
using TerminologyLauncher.GUI.ToolkitWindows.ProgressWindow;
using TerminologyLauncher.I18n;
using TerminologyLauncher.Utils.ProgressService;

namespace TerminologyLauncher.GUI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public sealed partial class LoginWindow : IPopup, INotifyPropertyChanged
    {
        private bool IsPerservePasswordValue;


        public delegate void LogingHandler(object serder, EventArgs e);

        public event LogingHandler Logining;

        public string LoginWindowTranslation => TranslationManager.GetManager.Localize("Title", "Terminology login");

        public string MojangAccountTranslation
            => TranslationManager.GetManager.Localize("MojangAccount", "Mojang Account:");

        public string OfflineAccountTranslation
            => TranslationManager.GetManager.Localize("OfflineAccount", "Offline Login:");

        public string MojangAccountModeTranslation
            => TranslationManager.GetManager.Localize("OfficialMode", "Official Mode");

        public string OfflineAccountModeTranslation
            => TranslationManager.GetManager.Localize("OfflineMode", "Offline Mode");

        public string PasswordTranslation => TranslationManager.GetManager.Localize("Password", "Password:");

        public string LoginModeTranslation => TranslationManager.GetManager.Localize("LoginMode", "Login Mode:");

        public string RememberAccountTranslation
            => TranslationManager.GetManager.Localize("RememberAccount", "Remember account");

        public string LoginButtonTranslation => TranslationManager.GetManager.Localize("LoginButtion", "Login");

        public string CancelButtonTranslation => TranslationManager.GetManager.Localize("CancelButton", "Cancel");

        private string BackgroundImageSourceValue;

        public string BackgroundImageSource
        {
            get { return this.BackgroundImageSourceValue; }
            set
            {
                this.BackgroundImageSourceValue = value;
                this.OnPropertyChanged();
            }
        }

        public Config Config { get; set; }

        public LoginWindow(Config config)
        {
            this.Config = config;

            if (!string.IsNullOrEmpty(this.Config.GetConfigString("loginWindowBackground")) &&
                File.Exists(this.Config.GetConfigString("loginWindowBackground")))
            {
                var imageFile = new FileInfo(this.Config.GetConfigString("loginWindowBackground"));
                this.BackgroundImageSource = imageFile.FullName;
            }
            else
            {
                this.BackgroundImageSource =
                    @"pack://application:,,,/TerminologyLauncher.GUI;component/Resources/login_bg.jpg";
            }

            this.InitializeComponent();
            this.OnPropertyChanged();
        }

        public void EnableAllInputs(bool isEnable)
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

        public bool IsPerservePassword
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
                    LoginType = (LoginType) this.LoginModeComboBox.SelectedIndex,
                    PerserveLogin = this.IsPerservePassword
                };
            });
            if (login == null)
            {
                throw new Exception("Cannot get valid login entity");
            }
            return login;
        }

        public void SetLogin(LoginEntity login)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.UsernameBox.Text = login.UserName;
                this.PasswordBox.Password = login.Password;
                this.LoginModeComboBox.SelectedIndex = (int) login.LoginType;
                this.IsPerservePassword = login.PerserveLogin;
            });
        }

        public void LoginResult(LoginResultType result)
        {
            this.Dispatcher.Invoke(() =>
            {
                var title = TranslationManager.GetManager.Localize("NotifyWindowTitle", "Unable to login in");
                switch (result)
                {
                    case LoginResultType.Success:
                    {
                        break;
                    }
                    case LoginResultType.IncompleteOfArguments:
                    {
                        this.PopupNotifyDialog(title,
                            TranslationManager.GetManager.Localize("InvalidInput",
                                "Password or username maybe invalid. Please check again."));
                        break;
                    }
                    case LoginResultType.WrongPassword:
                    {
                        this.PopupNotifyDialog(title,
                            TranslationManager.GetManager.Localize("WrongPasswrd", "Password is incorrect."));
                        break;
                    }
                    case LoginResultType.UserNotExists:
                    {
                        this.PopupNotifyDialog(title,
                            TranslationManager.GetManager.Localize("UserNotExists",
                                "User name is not exists. Create user before log in."));
                        break;
                    }
                    case LoginResultType.NetworkTimedOut:
                    {
                        this.PopupNotifyDialog(title,
                            TranslationManager.GetManager.Localize("NetWorkTimeOut",
                                "Can not access server. Please check network."));
                        break;
                    }
                    default:
                    {
                        this.PopupNotifyDialog(title,
                            TranslationManager.GetManager.Localize("UnknownLoginError",
                                "Unknown fault caused unable to log in."));
                        break;
                    }
                }
                this.EnableAllInputs(true);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoginMode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combox = sender as ComboBox;
            var selected = combox.SelectedIndex;
            this.AccountTypeTitle.Text = selected == 1
                ? TranslationManager.GetManager.Localize("MojangAccount", "Mojang Account:")
                : TranslationManager.GetManager.Localize("OfflineAccount", "Offline username:");

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

        private void OnLogining(object serder)
        {
            var handler = this.Logining;
            if (handler != null) handler(serder, EventArgs.Empty);
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) this.DragMove();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            this.OnLogining(this);
        }

        private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.OnLogining(this);
        }

        public void PopupNotifyDialog(string title, string content)
        {
            this.Dispatcher.Invoke(() =>
            {
                var notifyWindow = new NotifyWindow(this, title, content);
                notifyWindow.ShowDialog();
            });
        }

        public bool? PopupConfirmDialog(string title, string content)
        {
            throw new NotImplementedException();
        }

        public bool? PopupSingleSelectDialog(string title, string fieldName, IEnumerable<string> options,
            FieldReference<string> selection)
        {
            throw new NotImplementedException();
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