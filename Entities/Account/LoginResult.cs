using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.Entities.Account
{
    public enum LoginResult
    {
        UnknownError = 0,
        Success = 1,
        InsufficiencyOfArguments = 2,
        UserNotExists = 3,
        WrongPassword = 4,
        NetworkTimedOut = 5
    }
}
