using System;
using System.Runtime.Serialization;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaStepNotRegisteredException : Exception
    {
        public SagaStepNotRegisteredException(Guid id, string state, string step) :
            base($"Step {step} is not registered for state {state} (for saga {id})")
        {
        }

        protected SagaStepNotRegisteredException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}