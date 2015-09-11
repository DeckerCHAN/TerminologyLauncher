using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.InstanceManagerSystem.Exceptions
{
    public class NoAvailableUpdateException : Exception
    {
        public NoAvailableUpdateException()
        {
        }

        public NoAvailableUpdateException(string message)
            : base(message)
        {
        }

        public NoAvailableUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected NoAvailableUpdateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
