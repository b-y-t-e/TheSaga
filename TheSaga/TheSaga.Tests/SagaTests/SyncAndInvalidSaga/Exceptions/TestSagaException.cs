using System;

namespace TheSaga.Tests.SagaTests.SyncAndInvalidSaga.Exceptions
{
    [Serializable]
    public class TestSagaException : Exception
    {
        public TestSagaException()
        {
        }

        public TestSagaException(string message) : base(message)
        {
        }

        public TestSagaException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TestSagaException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
