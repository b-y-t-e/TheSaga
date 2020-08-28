using System;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class InvalidSagaModelNameException : Exception
    {
        public InvalidSagaModelNameException() :
            base($"Saga model name cannot be empty!")
        { }

        protected InvalidSagaModelNameException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}