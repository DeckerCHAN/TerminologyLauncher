using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TerminologyLauncher.InstanceManagerSystem.Exceptions
{
    public class WrongStateException : Exception
    {
        public WrongStateException()
        {
        }

        public WrongStateException(string message) : base(message)
        {
        }

        public WrongStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WrongStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
