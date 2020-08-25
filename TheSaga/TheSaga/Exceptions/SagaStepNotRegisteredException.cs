using System;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaStepNotRegisteredException : Exception
    {
        public SagaStepNotRegisteredException(string state, string step) :
            base($"Step {step} is not registered for state {state}")
        { }

        protected SagaStepNotRegisteredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}