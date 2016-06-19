using System;

namespace TerminologyLauncher.Entities.Account.Authentication.Refresh
{
    public class RefreshResponse : ResponseEntity
    {
        public string AccessToken { get; set; }
        public string ClientToken { get; set; }
        public ProfileEntity SelectedProfile { get; set; }
    }
}
