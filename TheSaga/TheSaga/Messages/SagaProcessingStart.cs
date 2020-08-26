using System;

namespace TheSaga.Messages
{
    internal class SagaProcessingStart : IInternalMessage
    {
        public SagaProcessingStart(Type sagaStateType, Guid correlationID)
        {
            SagaStateType = sagaStateType;
            CorrelationID = correlationID;
        }

        public Guid CorrelationID { get; }
        public Type SagaStateType { get; }
    }
}