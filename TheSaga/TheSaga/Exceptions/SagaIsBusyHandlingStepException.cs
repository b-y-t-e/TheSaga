using System;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaIsBusyHandlingStepException : Exception
    {
        public SagaIsBusyHandlingStepException(Guid id, string state, string step) :
            base($"Saga {id} in state {state} is busy (handling step {step})")
        { }

        protected SagaIsBusyHandlingStepException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}