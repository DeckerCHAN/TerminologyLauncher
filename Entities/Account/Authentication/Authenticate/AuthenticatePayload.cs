using System;

namespace TerminologyLauncher.Entities.Account.Authentication.Authenticate
{
    public class AuthenticatePayload
    {
        public AgentEntity Agent { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String ClientToken { get; set; }
    }
}
