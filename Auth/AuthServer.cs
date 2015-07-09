using System;
using System.IO;
using System.Reflection;
using System.Threading;
using TerminologyLauncher.Auth.Utils;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Logging;

namespace TerminologyLauncher.Auth
{
    public class AuthServer
    {
        public Config Config { get; set; }
        public Timer RefreshTimer { get; set; }
        public PlayerEntity CurrentPlayer { get; private set; }
        public DirectoryInfo ProfileDirectoryInfo { get; set; }
        public AuthServer()
        {
            Logger.GetLogger().Info("Initializing auth server.");
            this.Config = new Config(new FileInfo("Configs/AuthConfig.json"));
            //Create profile folder if not exists.
            this.ProfileDirectoryInfo = new DirectoryInfo(this.Config.GetConfig("profileFolder"));
            FolderUtils.RecreateFolder(this.ProfileDirectoryInfo);

            Logger.GetLogger().Info("Auth server initialized.");

        }


        public LoginResultEntity Auth(String username, String password)
        {
            return LoginResultEntity.UnknownError;
        }

        /// <summary>
        /// Support offline login.
        /// </summary>
        /// <param name="username">Username would be used in startup.</param>
        /// <returns>Login status(Should always be successful at offline mode.)</returns>
        public LoginResultEntity Auth(String username)
        {
            Logger.GetLogger().Debug(String.Format("Auth server authenticating user{0} in offline mode.", username));
            if (String.IsNullOrEmpty(username))
            {
                return LoginResultEntity.IncompleteOfArguments;
            }
            var userProfileFolder =
                new DirectoryInfo(Path.Combine(this.ProfileDirectoryInfo.FullName, username));

            FolderUtils.RecreateFolder(userProfileFolder);
            var userAvatarFileInfo = new FileInfo(Path.Combine(userProfileFolder.FullName, username));


            var myStream =TerminologyLauncher.Utils.ResourceUtils.ReadEmbedFileResource("TerminologyLauncher.Auth.Resources.default_large.png");


            var image = new FileStream(userAvatarFileInfo.FullName, FileMode.CreateNew);
            myStream.CopyTo(image);
            image.Flush();
            image.Close();

            this.CurrentPlayer = new PlayerEntity()
            {
                PlayerName = username,
                LoginMode = LoginModeEnum.OfficialMode,
                PlayerAvatarImagePath = userAvatarFileInfo.FullName

            };
            return LoginResultEntity.Success;
        }

        public void ShutDown()
        {

        }



    }
}
