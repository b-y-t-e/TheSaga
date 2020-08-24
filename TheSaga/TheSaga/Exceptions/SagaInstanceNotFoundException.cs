using System;
using System.Collections.Generic;
using System.Text;

namespace TheSaga.Exceptions
{

    [Serializable]
    public class SagaInstanceNotFoundException : Exception
    {
        public SagaInstanceNotFoundException(Type sagaStateType, Guid correlationID) :
            base($"Saga with correlationID {correlationID} not found (state type {sagaStateType.Name})!")
        { }
        protected SagaInstanceNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
