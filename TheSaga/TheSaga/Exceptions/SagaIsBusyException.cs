using System;
using System.Runtime.Serialization;

namespace TheSaga.Exceptions
{
    [Serializable]
    public class SagaIsBusyException : Exception
    {
        public SagaIsBusyException(Guid id) :
            base($"Saga {id} is busy")
        {
        }

        protected SagaIsBusyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}