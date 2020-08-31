using System;
using System.Runtime.Serialization;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaStepNotRegisteredException : Exception
    {
        public SagaStepNotRegisteredException(string state, string step) :
            base($"Step {step} is not registered for state {state}")
        {
        }

        protected SagaStepNotRegisteredException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}