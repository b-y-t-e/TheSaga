using System;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaIsBusyHandlingStepException : Exception
    {
        public SagaIsBusyHandlingStepException( Guid correlationID, string state, string step) :
            base($"Saga {correlationID} in state {state} is busy (handling step {step})")
        { }

        protected SagaIsBusyHandlingStepException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}