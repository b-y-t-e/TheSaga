using System;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaStepException : Exception
    {
        public SagaStepException(string message, string stackTrace) :
            base(message)
        {
            StackTrace = stackTrace;
        }

        protected SagaStepException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

        public string StackTrace { get; set; }
    }
}