using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.GUI;
using TerminologyLauncher.GUI.SingleLineInput;
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
                                var result = this.Engine.AuthServer.Auth(login.UserName, login.Password);
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
                                Logger.GetLogger().Error(String.Format("Auth encountered an error:{0}", ex.Message));
                                this.LoginFault(LoginResultType.NetworkTimedOut);
                            }
                        });




                        break;
                    }
                case LoginType.OfflineMode:
                    {
                        var result = this.Engine.AuthServer.Auth(login.UserName);
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
                        Logger.GetLogger().Error(String.Format("Core is not support {0} to login", login.LoginType));
                        break;
                    }
            }

        }

        private void LoginSuccess()
        {
            this.Engine.UiControl.LoginWindow.LoginResult(LoginResultType.Success);
            this.Engine.PostInitializeComponents();
            this.Engine.UiControl.MajorWindow.Player = this.Engine.AuthServer.CurrentPlayer;
            while (String.IsNullOrEmpty(this.Engine.InstanceManager.Config.GetConfig("javaPath")))
            {
                Logger.GetLogger().Warn("Java path is empty. Try to receive from user..");

                var result = this.Engine.UiControl.StartSingleLineInput("Request Java path", "Java Path");
                if (result.Type == SingleLineInputResultType.CommonFinished)
                {
                    if (String.IsNullOrEmpty(result.InputLine) || !new FileInfo(result.InputLine).Exists || (new FileInfo(result.InputLine).Name.ToLower() != "java.exe"))
                    {
                        //try again.
                    }
                    else
                    {
                        this.Engine.InstanceManager.Config.SetConfig("javaPath", result.InputLine);
                        Logger.GetLogger().Info("Received java path from user. Pass.");
                    }
                }
            }
            this.Engine.UiControl.ShowMainWindow();
        }

        private void LoginFault(LoginResultType reason)
        {
            this.Engine.UiControl.LoginWindow.LoginResult(reason);
        }
    }
}
