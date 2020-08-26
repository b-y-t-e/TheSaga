using System;

namespace TheSaga.InternalMessages
{
    internal class SagaProcessingStartMessage : IInternalMessage
    {
        public SagaProcessingStartMessage(Type sagaStateType, Guid correlationID)
        {
            SagaStateType = sagaStateType;
            CorrelationID = correlationID;
        }

        public Guid CorrelationID { get; }
        public Type SagaStateType { get; }
    }
}