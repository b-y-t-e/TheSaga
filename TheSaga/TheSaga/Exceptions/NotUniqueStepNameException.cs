using System;

namespace TheSaga.Exceptions
{

    [Serializable]
    public class NotUniqueStepNameException : Exception
    {
        public NotUniqueStepNameException() { }
        public NotUniqueStepNameException(string message) : base(message) { }
        public NotUniqueStepNameException(string message, Exception inner) : base(message, inner) { }
        protected NotUniqueStepNameException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}