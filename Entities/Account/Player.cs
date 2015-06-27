using System;

namespace TerminologyLauncher.Entities.Account
{
    public class Player
    {
        public String AccessToken { get; set; }
        public String ClientToken { get; set; }
        public LoginMode LoginMode { get; set; }
        public String PlayerName { get; set; }
        public String PlayerAvatarImagePath { get; set; }
    }
}
