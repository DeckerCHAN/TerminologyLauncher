using System;

namespace TerminologyLauncher.Entities.Account.Authentication
{
    public class AuthenticationResponseEntity
    {
        public String Error { get; set; }
        public String ErrorMessage { get; set; }
        public String Cause { get; set; }
    }
}
