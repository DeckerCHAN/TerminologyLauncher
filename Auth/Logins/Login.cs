using System;
using System.IO;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.Account;

namespace TerminologyLauncher.Auth.Logins
{
    public abstract class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        protected Config Config { get; set; }
        public DirectoryInfo ProfileRootDirectoryInfo { get; set; }
        public PlayerEntity Player { get; set; }

        protected Login(string userName, string password, DirectoryInfo profileRootDirectoryInfo,Config config)
        {
            this.UserName = userName;
            this.Password = password;
            this.Config = config;
            this.ProfileRootDirectoryInfo = profileRootDirectoryInfo;
        }

        public abstract LoginResultType ExecuteLogin();
    }
}
