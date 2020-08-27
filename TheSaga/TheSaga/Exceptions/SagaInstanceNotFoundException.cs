using System;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaInstanceNotFoundException : Exception
    {
        public SagaInstanceNotFoundException(Type sagaStateType, Guid id) :
            base($"Saga with id {id} not found (state type {sagaStateType.Name})!")
        { }

        public SagaInstanceNotFoundException(Guid id) :
            base($"Saga with id {id} not found!")
        { }

        protected SagaInstanceNotFoundException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}