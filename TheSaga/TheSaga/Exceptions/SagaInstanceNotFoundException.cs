using System;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaInstanceNotFoundException : Exception
    {
        public SagaInstanceNotFoundException(Type sagaStateType, Guid correlationID) :
            base($"Saga with correlationID {correlationID} not found (state type {sagaStateType.Name})!")
        { }

        public SagaInstanceNotFoundException(Guid correlationID) :
            base($"Saga with correlationID {correlationID} not found!")
        { }

        protected SagaInstanceNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}