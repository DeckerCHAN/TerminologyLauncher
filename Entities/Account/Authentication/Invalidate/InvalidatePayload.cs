using System;

namespace TerminologyLauncher.Entities.Account.Authentication.Invalidate
{
    public class InvalidatePayload
    {
        public String AccessToken { get; set; }
        public String ClientToken { get; set; }
    }
}
