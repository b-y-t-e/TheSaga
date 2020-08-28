using System;

namespace TheSaga.Exceptions
{
    internal class SagaStopException : Exception
    {
        public SagaStopException() : base()
        { }

        protected SagaStopException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}