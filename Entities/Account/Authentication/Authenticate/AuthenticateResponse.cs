using System;
using System.Collections.Generic;

namespace TerminologyLauncher.Entities.Account.Authentication.Authenticate
{
    public class AuthenticateResponse
    {
        public String AccessToken { get; set; }
        public String ClientToken { get; set; }
        public List<Profile> AvailableProfiles { get; set; }
        public List<Profile> SelectedProfile { get; set; }
    }
}
