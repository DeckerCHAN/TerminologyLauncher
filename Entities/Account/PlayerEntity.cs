using System;

namespace TerminologyLauncher.Entities.Account
{
    public class PlayerEntity
    {
        public String AccessToken { get; set; }
        public String ClientToken { get; set; }
        public LoginType LoginType { get; set; }
        public String PlayerId { get; set; }
        public String PlayerName { get; set; }
        public String PlayerAvatarImagePath { get; set; }
    }
}
