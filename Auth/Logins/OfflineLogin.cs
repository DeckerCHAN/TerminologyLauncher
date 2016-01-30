using System;
using System.IO;
using System.Text.RegularExpressions;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Utils;

namespace TerminologyLauncher.Auth.Logins
{
    public class OfflineLogin : Login
    {

        public String PlayerId { get; set; }

        private Boolean isValidUserName(String userName)
        {
            var regex = new Regex(@"^[a-zA-Z0-9]+$");
            var match = regex.Match(userName);
            return match.Success;
        }

        public override LoginResultType ExecuteLogin()
        {
            if (String.IsNullOrEmpty(this.UserName) || this.UserName.Length < 4||!this.isValidUserName(this.UserName))
            {
                return LoginResultType.IncompleteOfArguments;
            }
            var userProfileFolder =
              new DirectoryInfo(Path.Combine(this.ProfileRootDirectoryInfo.FullName, this.UserName));
            FolderUtils.CreateDirectoryIfNotExists(userProfileFolder);

            var userAvatarFileInfo = new FileInfo(Path.Combine(userProfileFolder.FullName, this.UserName + ".png"));
            var defaultImageStream = ResourceUtils.ReadEmbedFileResource("TerminologyLauncher.Auth.Resources.Avatar.png");
            using (var image = new FileStream(userAvatarFileInfo.FullName, FileMode.CreateNew))
            {
                defaultImageStream.CopyTo(image);
                image.Flush();
                image.Close();
            }



            this.Player = new PlayerEntity()
            {
                PlayerName = this.UserName,
                PlayerId = EncodeUtils.CalculateStringMd5(this.UserName),
                AccessToken = Guid.NewGuid().ToString("N"),
                ClientToken = Guid.NewGuid().ToString("N"),
                LoginType = LoginType.OfflineMode,
                PlayerAvatarImagePath = userAvatarFileInfo.FullName

            };
            return LoginResultType.Success;
        }

        public OfflineLogin(string userName, DirectoryInfo profileRootDirectoryInfo, Config config)
            : base(userName, String.Empty, profileRootDirectoryInfo, config)
        {
            this.PlayerId = EncodeUtils.CalculateStringMd5(userName);
        }
    }
}
