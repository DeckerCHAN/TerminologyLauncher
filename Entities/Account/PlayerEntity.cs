using System;

namespace TerminologyLauncher.Entities.Account
{
    public class PlayerEntity
    {
        public String AccessToken { get; set; }
        public String ClientToken { get; set; }
        public LoginModeEnum LoginMode { get; set; }
        public Guid PlayerId { get; set; }
        public String PlayerName { get; set; }
        public String PlayerAvatarImagePath { get; set; }
    }
}
