using System;

namespace TerminologyLauncher.Entities.Account
{
    public class ResponseEntity
    {
        public string Error { get; set; }
        public string ErrorMessage { get; set; }
        public string Cause { get; set; }
    }
}