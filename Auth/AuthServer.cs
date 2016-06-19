using System;
using System.IO;
using System.Threading;
using TerminologyLauncher.Auth.Logins;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Logging;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.Auth
{
    public class AuthServer
    {
        public Config Config { get; set; }
        public Timer RefreshTimer { get; set; }
        public PlayerEntity CurrentPlayer { get; private set; }
        public DirectoryInfo ProfileDirectoryInfo { get; set; }
        public AuthServer(string authConfigPath)
        {
            TerminologyLogger.GetLogger().Info("Initializing auth server.");
            this.Config = new Config(new FileInfo(authConfigPath));
            //Create profile folder if not exists.
            this.ProfileDirectoryInfo = new DirectoryInfo(this.Config.GetConfigString("profileFolder"));
            FolderUtils.RecreateFolder(this.ProfileDirectoryInfo);

            TerminologyLogger.GetLogger().Info("Auth server initialized.");

        }



        public LoginResultType OfficialAuth(string username, string password)
        {
            TerminologyLogger.GetLogger().Debug($"Auth server authenticating user{username} in official mode.");
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

        public LoginResultType OfflineAuth(string username)
        {
            TerminologyLogger.GetLogger().Debug($"Auth server authenticating user{username} in offline mode.");
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
