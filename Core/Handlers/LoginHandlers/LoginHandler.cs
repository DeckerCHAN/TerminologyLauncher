using System;
using System.IO;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.GUI;
using TerminologyLauncher.GUI.SingleLineInputWindow;
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
            while (String.IsNullOrEmpty(this.Engine.InstanceManager.Config.GetConfig("javaPath")))
            {
                Logger.GetLogger().Warn("Java path is empty. Try to receive from user..");

                var result = new SingleLineInputWindow("Request Java path", "Java Path").ReceiveUserinput();
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
            this.Engine.UiControl.MainWindow.Show();
        }

        private void LoginFault(LoginResultEntity reason)
        {
            this.Engine.UiControl.LoginWindow.LoginResult(reason);
        }
    }
}
