using System;
using System.Runtime.Serialization;

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
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public string StackTrace { get; set; }
    }
}