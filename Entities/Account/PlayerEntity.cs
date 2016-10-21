using System;

namespace TerminologyLauncher.Entities.Account
{
    public class PlayerEntity
    {
        public string AccessToken { get; set; }
        public string ClientToken { get; set; }
        public LoginType LoginType { get; set; }
        public string PlayerId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerAvatarImagePath { get; set; }
    }
}