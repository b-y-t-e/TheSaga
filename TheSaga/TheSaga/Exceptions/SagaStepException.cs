using System;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaStepException : Exception
    {
        public SagaStepException(Guid correlationID, String currentState, String currentError) :
            base($"{currentError}")
        {
            CorrelationID = correlationID;
            CurrentState = currentState;
            CurrentError = currentError;
        }

        protected SagaStepException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public Guid CorrelationID { get; }
        public string CurrentError { get; }
        public string CurrentState { get; }
    }
}