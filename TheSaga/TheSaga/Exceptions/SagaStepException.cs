using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaStepException : Exception
    {
        [JsonIgnore]
        public Exception OriginalException { get; set; }

        public SagaStepException(string message, string stackTrace, Exception originalException) :
            base(message)
        {
            this.StackTrace = stackTrace;
            this.OriginalException = originalException;
        }

        protected SagaStepException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public string StackTrace { get; set; }
    }
}