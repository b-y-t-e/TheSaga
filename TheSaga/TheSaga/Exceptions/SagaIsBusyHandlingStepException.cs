using System;
using System.Runtime.Serialization;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaIsBusyHandlingStepException : Exception
    {
        public SagaIsBusyHandlingStepException(Guid id, string state, string step) :
            base($"Saga {id} in state {state} is busy (handling step {step})")
        {
        }

        protected SagaIsBusyHandlingStepException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}