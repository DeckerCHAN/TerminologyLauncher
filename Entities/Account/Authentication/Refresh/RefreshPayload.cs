using System;

namespace TerminologyLauncher.Entities.Account.Authentication.Refresh
{
    public class RefreshPayload
    {
        public string AccessToken { get; set; }
        public string ClientToken { get; set; }
        public ProfileEntity SelectedProfile { get; set; }
    }
}
