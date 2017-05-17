using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TerminologyLauncher.Configs;
using TerminologyLauncher.Entities.Account;
using TerminologyLauncher.Entities.Account.AdditionalInfo;
using TerminologyLauncher.Entities.Account.AdditionalInfo.Properties;
using TerminologyLauncher.Entities.Account.Authentication.Authenticate;
using TerminologyLauncher.Utils;
using TerminologyLauncher.Utils.SerializeUtils;

namespace TerminologyLauncher.Auth.Logins
{
    class MojangLogin : Login
    {
        public MojangLogin(string userName, string password, DirectoryInfo profileRootDirectoryInfo, Config config)
            : base(userName, password, profileRootDirectoryInfo, config)
        {
        }

        private Stream ReceiveUserAvatar(AdditionalInfoEntity additionalInfo)
        {
            var defaultImageStream = ResourceUtils.ReadEmbedFileResource("TerminologyLauncher.Auth.Resources.Avatar.png");
            try
            {
                var value = additionalInfo.Properties[0].Value;
                var decode = EncodeUtils.Base64Decode(value);
                var textures = JsonConverter.Parse<TexturesInfoEntity>(decode);
                var url = textures.Textures.SKIN.Url;

                var request = (HttpWebRequest) WebRequest.Create(url);
                var orgStream = new MemoryStream();
                request.GetResponse().GetResponseStream().CopyTo(orgStream);
                orgStream.Seek(0, SeekOrigin.Begin);
                var cropStream = this.SelectImage(orgStream);
                cropStream.Seek(0, SeekOrigin.Begin);
                return cropStream;
            }
            catch (Exception)
            {
                return defaultImageStream;
            }
        }

        public override LoginResultType ExecuteLogin()
        {
            if (!this.IsValidMailAddress(this.UserName) || string.IsNullOrEmpty(this.Password))
            {
                return LoginResultType.IncompleteOfArguments;
            }
            try
            {
                var authResponse = this.ReceiveUserAuthenticateResponse();

                var additionalInfo = this.ReceiveAdditionalInfoEntity(authResponse.SelectedProfile.Id);


                var userDiectionary =
                    new DirectoryInfo(Path.Combine(this.ProfileRootDirectoryInfo.FullName,
                        authResponse.SelectedProfile.Id));
                FolderUtils.RecreateFolder(userDiectionary);
                var userAvatarFileInfo =
                    new FileInfo(Path.Combine(userDiectionary.FullName, authResponse.SelectedProfile.Name + ".png"));
                using (var fileStream = new FileStream(userAvatarFileInfo.FullName, FileMode.Create))
                {
                    this.ReceiveUserAvatar(additionalInfo).CopyTo(fileStream);
                    fileStream.Flush();
                    fileStream.Close();
                }

                this.Player = new PlayerEntity()
                {
                    AccessToken = authResponse.AccessToken,
                    ClientToken = authResponse.ClientToken,
                    LoginType = LoginType.OfficialMode,
                    PlayerAvatarImagePath = userAvatarFileInfo.FullName,
                    PlayerName = authResponse.SelectedProfile.Name,
                    PlayerId = authResponse.SelectedProfile.Id
                };

                return LoginResultType.Success;
            }
            catch (WebException)
            {
                return LoginResultType.NetworkTimedOut;
            }
            catch
            {
                return LoginResultType.UnknownError;
            }
        }


        private AdditionalInfoEntity ReceiveAdditionalInfoEntity(string id)
        {
            var response = DownloadUtils.GetWebContent(this.Config.GetConfigString("profileUrl") + id);
            return JsonConverter.Parse<AdditionalInfoEntity>(response);
        }

        public Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = SmoothingMode.None;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private Stream SelectImage(Stream stream)
        {
            var tarStream = new MemoryStream();

            var bitmap = new Bitmap(8, 8);

            var g = Graphics.FromImage(bitmap);

            g.DrawImage(new Bitmap(stream), 0, 0, new Rectangle(8, 8, 8, 8), GraphicsUnit.Pixel);

            bitmap = this.ResizeImage(bitmap, 50, 50);

            bitmap.Save(tarStream, ImageFormat.Png);
            tarStream.Seek(0, SeekOrigin.Begin);
            return tarStream;
        }

        private AuthenticateResponse ReceiveUserAuthenticateResponse()
        {
            var sendPayload = new AuthenticatePayload()
            {
                Agent = new AgentEntity()
                {
                    Name = "Minecraft",
                    Version = 1
                },
                Username = this.UserName,
                Password = this.Password
            };

            string authResponse = string.Empty;

            var request = (HttpWebRequest) WebRequest.Create(this.Config.GetConfigString("authUrls.authenticate"));
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json";
            var postData = JsonConverter.ConvertToJson(sendPayload);
            var postByteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = postByteArray.Length;
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(postByteArray, 0, postByteArray.Length);
            }
            var response = request.GetResponse();


            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                    using (var responseReader = new StreamReader(responseStream))
                    {
                        authResponse = responseReader.ReadToEnd();
                    }
            }


            return JsonConverter.Parse<AuthenticateResponse>(authResponse);
        }

        private bool IsValidMailAddress(string emailaddress)
        {
            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            var match = regex.Match(emailaddress);
            return match.Success;
        }
    }
}