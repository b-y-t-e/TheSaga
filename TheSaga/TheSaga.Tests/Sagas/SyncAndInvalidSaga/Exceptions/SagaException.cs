using System;

namespace TheSaga.Tests.Sagas.SyncAndInvalidSaga.Exceptions
{

    [Serializable]
    public class SagaException : Exception
    {
        public SagaException() { }
        public SagaException(string message) : base(message) { }
        public SagaException(string message, Exception inner) : base(message, inner) { }
        protected SagaException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}