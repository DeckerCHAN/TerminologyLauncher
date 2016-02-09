using System;
using System.Net;
using System.Threading.Tasks;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.Core.Handlers.LoginHandlers
{
    public class LoginHandlerBase : HandlerBase
    {
        public LoginHandlerBase(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.LoginWindow.Logining += this.HandleEvent;
        }
        public override void HandleEvent(Object sender, EventArgs e)
        {
            this.Engine.UiControl.LoginWindow.EnableAllInputs(false);
            var login = this.Engine.UiControl.LoginWindow.GetLogin();
            switch (login.LoginType)
            {
                case LoginType.OfficialMode:
                    {

                        if (String.IsNullOrEmpty(login.UserName) || String.IsNullOrEmpty(login.Password))
                        {
                            this.LoginFault(LoginResultType.IncompleteOfArguments);
                            return;
                        }

                        var t = Task.Run(() =>
                        {
                            try
                            {
                                var result = this.Engine.AuthServer.OfficialAuth(login.UserName, login.Password);
                                if (result == LoginResultType.Success)
                                {
                                    this.LoginSuccess();
                                }
                                else
                                {
                                    this.LoginFault(result);
                                }
                            }
                            catch (WebException ex)
                            {
                                TerminologyLogger.GetLogger().Error(String.Format("OfficialAuth encountered an error:{0}", ex.Message));
                                this.LoginFault(LoginResultType.NetworkTimedOut);
                            }
                        });




                        break;
                    }
                case LoginType.OfflineMode:
                    {
                        var result = this.Engine.AuthServer.OfflineAuth(login.UserName);
                        if (result == LoginResultType.Success)
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
                        TerminologyLogger.GetLogger().Error(String.Format("Core is not support {0} to login", login.LoginType));
                        break;
                    }
            }

        }

        private void LoginSuccess()
        {
            this.Engine.UiControl.LoginWindow.LoginResult(LoginResultType.Success);
            this.Engine.UiControl.HideLoginWindow();
            this.Engine.PostInitializeComponents();
            this.Engine.UiControl.MainWindow.Player = this.Engine.AuthServer.CurrentPlayer;
            this.Engine.UiControl.ShowMainWindow();
        }

        private void LoginFault(LoginResultType reason)
        {
            this.Engine.UiControl.LoginWindow.LoginResult(reason);
        }


    }
}
