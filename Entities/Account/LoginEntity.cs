using System;

namespace TerminologyLauncher.Entities.Account
{
    public class LoginEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public LoginType LoginType { get; set; }
        public bool PerserveLogin { get; set; }
    }
}