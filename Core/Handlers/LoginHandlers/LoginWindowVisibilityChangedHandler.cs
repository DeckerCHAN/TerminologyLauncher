using System;
using System.Windows;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.Core.Handlers.LoginHandlers
{
    public class LoginWindowVisibilityChangedHandler : IHandler
    {
        public void HandleEvent(Object sender, DependencyPropertyChangedEventArgs e)
        {
            Logger.GetLogger().Debug("Handle login window show or hide at here.");
            var window = sender as Window;
            Logger.GetLogger().Debug(String.Format("Login window changed status to {0}", window.Visibility));
            
        }

        public void HandleEvent(object sender, EventArgs e)
        {
            throw new NotSupportedException();
        }
    }
}
