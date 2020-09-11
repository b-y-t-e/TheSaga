using System;

namespace TheSaga.Tests.SagaTests.SyncAndInvalidSaga.Exceptions
{
    [Serializable]
    public class TestCompensationException : Exception
    {
        public TestCompensationException()
        {
        }

        public TestCompensationException(string message) : base(message)
        {
        }

        public TestCompensationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TestCompensationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
