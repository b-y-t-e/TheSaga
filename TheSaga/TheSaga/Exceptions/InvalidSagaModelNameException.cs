using System;
using System.Runtime.Serialization;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class InvalidSagaModelNameException : Exception
    {
        public InvalidSagaModelNameException() :
            base("Saga model name cannot be empty!")
        {
        }

        protected InvalidSagaModelNameException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}