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
            Id = id;
        }

        protected SagaIsBusyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public Guid Id { get; }
    }
}