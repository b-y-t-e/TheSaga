using System;
using System.Runtime.Serialization;

namespace TheSaga.Exceptions
{

    [Serializable]
    public class CountNotExecuteDeletedSagaException : Exception
    {
        public CountNotExecuteDeletedSagaException(Guid id) :
            base($"Count not execute deleted saga {id}!")
        {
        }

        protected CountNotExecuteDeletedSagaException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class SagaInstanceNotFoundException : Exception
    {
        public SagaInstanceNotFoundException(Type sagaStateType, Guid id) :
            base($"Saga with id {id} not found (state type {sagaStateType.Name})!")
        {
        }

        public SagaInstanceNotFoundException(Type sagaStateType) :
            base($"Saga not found (state type {sagaStateType.Name})!")
        {
        }

        public SagaInstanceNotFoundException(Guid id) :
            base($"Saga with id {id} not found!")
        {
        }


        protected SagaInstanceNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}