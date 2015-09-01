using System;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.Core.Handlers.LoginHandlers
{
    public class LoginHandlerBase : HandlerBase
    {
        public LoginHandlerBase(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.LoginWindow.LoginButton.Click += this.HandleEvent;
        }
        public override void HandleEvent(Object sender, EventArgs e)
        {
            this.Engine.UiControl.LoginWindow.EnableAllInputs(false);
            var login = this.Engine.UiControl.LoginWindow.GetLogin();
            switch (login.LoginMode)
            {
                case LoginModeEnum.OfficialMode:
                    {
                        throw new NotImplementedException();
                        if (String.IsNullOrEmpty(login.UserName) || String.IsNullOrEmpty(login.Password))
                        {
                            this.LoginFault(LoginResultEntity.IncompleteOfArguments);
                            return;
                        }


                        break;
                    }
                case LoginModeEnum.OfflineMode:
                    {
                        var result = this.Engine.AuthServer.Auth(login.UserName);
                        if (result == LoginResultEntity.Success)
                        {
                            this.LoginSuccess();
                        }
                        else
                        {
                            this.LoginFault(result);
                        }
                        break;
                    }

                default:
                    {
                        Logger.GetLogger().Error(String.Format("Core is not support {0} to login", login.LoginMode));
                        break;
                    }
            }

        }

        private void LoginSuccess()
        {
            this.Engine.UiControl.LoginWindow.LoginResult(LoginResultEntity.Success);
            this.Engine.PostInitializeComponents();
            this.Engine.UiControl.MajorWindow.Player = this.Engine.AuthServer.CurrentPlayer;
            this.Engine.UiControl.MainWindow.Show();
        }

        private void LoginFault(LoginResultEntity reason)
        {
            this.Engine.UiControl.LoginWindow.LoginResult(reason);
        }
    }
}
