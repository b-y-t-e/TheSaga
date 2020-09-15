using System;
using System.Runtime.Serialization;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaInvalidEventForStateException : Exception
    {
        public SagaInvalidEventForStateException(Guid id, string currentState, Type eventType) :
            base(
                $"Saga {id} in state {currentState} does not have support for event of type {(eventType != null ? eventType.Name : "null")}!")
        {
        }

        protected SagaInvalidEventForStateException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}