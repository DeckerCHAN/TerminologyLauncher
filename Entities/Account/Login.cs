using System;

namespace TerminologyLauncher.Entities.Account
{
    public class Login
    {
        public String UserName { get; set; }
        public String Password { get; set; }
        public LoginMode LoginMode { get; set; }
    }
}
