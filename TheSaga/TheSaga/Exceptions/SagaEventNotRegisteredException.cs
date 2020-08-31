using System;
using System.Runtime.Serialization;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaEventNotRegisteredException : Exception
    {
        public SagaEventNotRegisteredException(Type eventType) :
            base($"Event of type {eventType.Name} is not registered")
        {
        }

        protected SagaEventNotRegisteredException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}