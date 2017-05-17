using System;
using TerminologyLauncher.Utils.Exceptions;

namespace TerminologyLauncher.Updater.Exceptions.Update
{
    public class UpdateServerErrorException : SolutionProvidedException
    {
        public UpdateServerErrorException(string message)
            : base(message, "Try to reinstall or mail to DeckerCHAN@gmail.com to report this problem.")
        {
        }
    }
}