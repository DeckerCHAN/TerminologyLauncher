using System;

namespace TerminologyLauncher.Entities.Account
{
    public class ResponseEntity
    {
        public String Error { get; set; }
        public String ErrorMessage { get; set; }
        public String Cause { get; set; }
    }
}
