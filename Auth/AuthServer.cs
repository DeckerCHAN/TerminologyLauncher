using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using TerminologyLauncher.Auth.Logins;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Entities.Account.Authentication.Authenticate;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.SerializeUtils;

namespace TerminologyLauncher.Auth
{
    public class AuthServer
    {
        public Config Config { get; set; }
        public Timer RefreshTimer { get; set; }
        public PlayerEntity CurrentPlayer { get; private set; }
        public DirectoryInfo ProfileDirectoryInfo { get; set; }
        public AuthServer(String authConfigPath)
        {
            TerminologyLogger.GetLogger().Info("Initializing auth server.");
            this.Config = new Config(new FileInfo(authConfigPath));
            //Create profile folder if not exists.
            this.ProfileDirectoryInfo = new DirectoryInfo(this.Config.GetConfigString("profileFolder"));
            FolderUtils.RecreateFolder(this.ProfileDirectoryInfo);

            TerminologyLogger.GetLogger().Info("Auth server initialized.");

        }



        public LoginResultType OfficialAuth(String username, String password)
        {
            TerminologyLogger.GetLogger().Debug(String.Format("Auth server authenticating user{0} in official mode.", username));
            var mojangLogin = new MojangLogin(username, password, this.ProfileDirectoryInfo, this.Config);
            var result = mojangLogin.ExecuteLogin();
            if (result == LoginResultType.Success)
            {
                this.CurrentPlayer = mojangLogin.Player;
            }
            return result;
        }

        public LoginResultType TerminologyAuth()
        {
            throw new NotImplementedException();
        }

        public LoginResultType OfflineAuth(String username)
        {
            TerminologyLogger.GetLogger().Debug(String.Format("Auth server authenticating user{0} in offline mode.", username));
            var offlineLogin = new OfflineLogin(username, this.ProfileDirectoryInfo, this.Config);
            var result = offlineLogin.ExecuteLogin();
            if (result == LoginResultType.Success)
            {
                this.CurrentPlayer = offlineLogin.Player;
            }
            return result;
        }

        public void ShutDown()
        {

        }



    }
}
