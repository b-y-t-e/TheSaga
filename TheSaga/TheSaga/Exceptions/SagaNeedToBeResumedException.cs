using System;
using System.Runtime.Serialization;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaNeedToBeResumedException : Exception
    {
        public SagaNeedToBeResumedException(Guid id) :
            base($"Saga {id} need to be resumed")
        {
        }

        protected SagaNeedToBeResumedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}