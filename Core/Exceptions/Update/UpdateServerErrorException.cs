using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TerminologyLauncher.Utils.Exceptions;

namespace TerminologyLauncher.Core.Exceptions.Update
{
    public class UpdateServerErrorException:SolutionProvidedException
    {
        public UpdateServerErrorException() : base("Encounter error during fecth update from server.", "Please mail to DeckerCHAN@gmail.com to report this problem.")
        {
        }
    }
}
