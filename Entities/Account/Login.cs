using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.Entities.Account
{
    public class Login
    {
        public String UserName { get; set; }
        public String Password { get; set; }
        public LoginMode LoginMode { get; set; }
    }
}
