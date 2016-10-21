using System;
using System.Runtime.Serialization;

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