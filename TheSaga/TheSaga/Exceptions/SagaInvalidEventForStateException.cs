using System;
using System.Collections.Generic;
using System.Text;

namespace TheSaga.Exceptions
{

    [Serializable]
    public class SagaInvalidEventForStateException : Exception
    {
        public SagaInvalidEventForStateException(string currentState, Type eventType) :
            base($"Saga in state {currentState} does not have support for event of type {(eventType != null ? eventType.Name : "null")}!")
        { }
        protected SagaInvalidEventForStateException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
