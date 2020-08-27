using System;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaIsBusyException : Exception
    {
        public SagaIsBusyException(Guid id) :
            base($"Saga {id} is busy")
        { }

        protected SagaIsBusyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}