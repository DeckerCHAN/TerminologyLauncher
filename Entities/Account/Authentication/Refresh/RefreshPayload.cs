using System;

namespace TerminologyLauncher.Entities.Account.Authentication.Refresh
{
    public class RefreshPayload
    {
        public String AccessToken { get; set; }
        public String ClientToken { get; set; }
        public Profile SelectedProfile { get; set; }
    }
}
