using System;
using System.Collections.Generic;
using System.Text;

namespace TheSaga.Exceptions
{

    [Serializable]
    public class SagaEventNotRegisteredException : Exception
    {
        public SagaEventNotRegisteredException(Type eventType) :
            base($"Event of type {eventType.Name} is not registered")
        { }
        protected SagaEventNotRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
