using System;
using System.Windows;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.Core.Handlers.LoginHandlers
{
    public class LoginWindowVisibilityChangedHandler : HandlerBase
    {
        public void HandleEvent(Object sender, DependencyPropertyChangedEventArgs e)
        {
            var window = sender as Window;
            Logger.GetLogger().InfoFormat("Login window is going to {0}!", window.Visibility);

        }

        public override void HandleEvent(object sender, EventArgs e)
        {
            throw new NotSupportedException();
        }

        public LoginWindowVisibilityChangedHandler(Engine engine)
            : base(engine)
        {
            this.Engine.UiControl.LoginWindow.IsVisibleChanged += this.HandleEvent;
        }
    }
}
