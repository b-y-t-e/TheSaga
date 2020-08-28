using System;
using System.Runtime.Serialization;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class NotUniqueStepNameException : Exception
    {
        public NotUniqueStepNameException()
        {
        }

        public NotUniqueStepNameException(string message) : base(message)
        {
        }

        public NotUniqueStepNameException(string message, Exception inner) : base(message, inner)
        {
        }

        protected NotUniqueStepNameException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}