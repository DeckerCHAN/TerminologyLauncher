using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Entities.Account.Authentication.Authenticate;
using TerminologyLauncher.Entities.SerializeUtils;
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
        public AuthServer(String authConfigPath)
        {
            Logger.GetLogger().Info("Initializing auth server.");
            this.Config = new Config(new FileInfo(authConfigPath));
            //Create profile folder if not exists.
            this.ProfileDirectoryInfo = new DirectoryInfo(this.Config.GetConfig("profileFolder"));
            FolderUtils.RecreateFolder(this.ProfileDirectoryInfo);

            Logger.GetLogger().Info("Auth server initialized.");

        }


        public LoginResultType Auth(String username, String password)
        {
            var sendPayload = new AuthenticatePayload()
            {
                Agent = new AgentEntity()
                {
                    Name = "Minecraft",
                    Version = 1
                },
                Username = username,
                Password = password
            };

            String authResponse;
            using (var client = new WebClient())
            {
                authResponse = client.UploadString(this.Config.GetConfig("authUrls.authenticate"),
                    JsonConverter.ConvertToJson(sendPayload));
            }



            var responseObj = JsonConverter.Parse<AuthenticateResponse>(authResponse);

            if (!String.IsNullOrEmpty(responseObj.ErrorMessage))
            {
                return LoginResultType.UnknownError;
            }

            var userProfileFolder =
          new DirectoryInfo(Path.Combine(this.ProfileDirectoryInfo.FullName, username));
            FolderUtils.RecreateFolder(userProfileFolder);

            var userAvatarFileInfo = new FileInfo(Path.Combine(userProfileFolder.FullName, username + ".png"));
            var myStream = ResourceUtils.ReadEmbedFileResource("TerminologyLauncher.Auth.Resources.Avatar.png");
            var image = new FileStream(userAvatarFileInfo.FullName, FileMode.CreateNew);
            myStream.CopyTo(image);
            image.Flush();
            image.Close();


            this.CurrentPlayer = new PlayerEntity()
            {
                AccessToken = responseObj.AccessToken,
                ClientToken = responseObj.ClientToken,
                LoginType = LoginType.OfficialMode,
                PlayerAvatarImagePath = userAvatarFileInfo.FullName,
                PlayerName = responseObj.SelectedProfile.Name,
                PlayerId = responseObj.SelectedProfile.Id

            };

            return LoginResultType.Success;
        }

        /// <summary>
        /// Support offline login.
        /// </summary>
        /// <param name="username">Username would be used in startup.</param>
        /// <returns>Login status(Should always be successful at offline mode.)</returns>
        public LoginResultType Auth(String username)
        {
            Logger.GetLogger().Debug(String.Format("Auth server authenticating user{0} in offline mode.", username));
            if (String.IsNullOrEmpty(username))
            {
                return LoginResultType.IncompleteOfArguments;
            }
            var userProfileFolder =
                new DirectoryInfo(Path.Combine(this.ProfileDirectoryInfo.FullName, username));

            FolderUtils.RecreateFolder(userProfileFolder);
            var userAvatarFileInfo = new FileInfo(Path.Combine(userProfileFolder.FullName, username + ".png"));
            var myStream = ResourceUtils.ReadEmbedFileResource("TerminologyLauncher.Auth.Resources.Avatar.png");
            var image = new FileStream(userAvatarFileInfo.FullName, FileMode.CreateNew);
            myStream.CopyTo(image);
            image.Flush();
            image.Close();

            this.CurrentPlayer = new PlayerEntity()
            {
                PlayerName = username,
                LoginType = LoginType.OfflineMode,
                PlayerAvatarImagePath = userAvatarFileInfo.FullName

            };
            return LoginResultType.Success;
        }

        public void ShutDown()
        {

        }



    }
}
