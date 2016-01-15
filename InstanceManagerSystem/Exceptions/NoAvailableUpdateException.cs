using System;
using System.Runtime.Serialization;

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
