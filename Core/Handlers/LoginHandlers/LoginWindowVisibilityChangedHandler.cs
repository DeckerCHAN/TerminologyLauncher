using System;
using System.IO;
using System.Text;
using System.Windows;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Entities.SerializeUtils;
using TerminologyLauncher.GUI;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.Core.Handlers.LoginHandlers
{
    public class LoginWindowVisibilityChangedHandler : HandlerBase
    {

        public LoginWindowVisibilityChangedHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.LoginWindow.IsVisibleChanged += this.HandleEvent;
        }
        public void HandleEvent(Object sender, DependencyPropertyChangedEventArgs e)
        {
            var window = sender as LoginWindow;
            Logger.GetLogger().InfoFormat("Login window is going to {0}!", window.Visibility);
            switch (window.Visibility)
            {
                case Visibility.Hidden:
                    {
                        if (window.IsPerservePassword)
                        {

                            var bin =
                                Encoding.ASCII.GetBytes(
                                    EncodeUtils.Base64Encode(JsonConverter.ConvertToJson(window.GetLogin())));
                            File.WriteAllBytes(this.Engine.CoreConfig.GetConfig("loginPerserveFilePath"), bin);

                        }
                        else
                        {
                            if (File.Exists(this.Engine.CoreConfig.GetConfig("loginPerserveFilePath")))
                            {
                                File.Delete(this.Engine.CoreConfig.GetConfig("loginPerserveFilePath"));
                            }
                        }
                        break;
                    }
                case Visibility.Visible:
                    {
                        if (File.Exists(this.Engine.CoreConfig.GetConfig("loginPerserveFilePath")))
                        {

                            var bin = File.ReadAllBytes(this.Engine.CoreConfig.GetConfig("loginPerserveFilePath"));
                            var login = JsonConverter.Parse<LoginEntity>(EncodeUtils.Base64Decode(Encoding.ASCII.GetString(bin)));
                            window.SetLogin(login);
                        }
                        break;
                    }
            }
        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            throw new NotSupportedException();
        }
    }
}
