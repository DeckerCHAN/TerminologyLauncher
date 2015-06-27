using System;
using System.Collections.Generic;

namespace TerminologyLauncher.Entities.Account.Authentication.Authenticate
{
    public class AuthenticateResponse
    {
        public String AccessToken { get; set; }
        public String ClientToken { get; set; }
        public List<ProfileEntity> AvailableProfiles { get; set; }
        public List<ProfileEntity> SelectedProfile { get; set; }
    }
}
