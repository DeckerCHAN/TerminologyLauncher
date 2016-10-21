using System;

namespace TerminologyLauncher.Entities.Account.Authentication.Invalidate
{
    public class InvalidatePayload
    {
        public string AccessToken { get; set; }
        public string ClientToken { get; set; }
    }
}