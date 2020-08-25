using System;

namespace TheSaga.Messages
{
    internal class SagaProcessingEnd : IInternalMessage
    {
        public SagaProcessingEnd(Type sagaStateType, Guid correlationID)
        {
            SagaStateType = sagaStateType;
            CorrelationID = correlationID;
        }

        public Guid CorrelationID { get; }
        public Type SagaStateType { get; }
    }
}