using System;

namespace TerminologyLauncher.Entities.Account.Authentication.Refresh
{
    public class RefreshResponse : ResponseEntity
    {
        public String AccessToken { get; set; }
        public String ClientToken { get; set; }
        public ProfileEntity SelectedProfile { get; set; }
    }
}
