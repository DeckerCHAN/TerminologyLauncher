using System;

namespace TerminologyLauncher.Entities.Account.Authentication.Authenticate
{
    public class AuthenticatePayload
    {
        public AgentEntity Agent { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientToken { get; set; }
    }
}