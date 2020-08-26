using System;

namespace TheSaga.InternalMessages
{
    internal class SagaProcessingCompletedMessage : IInternalMessage
    {
        public SagaProcessingCompletedMessage(Type sagaStateType, Guid correlationID)
        {
            SagaStateType = sagaStateType;
            CorrelationID = correlationID;
        }

        public Guid CorrelationID { get; }
        public Type SagaStateType { get; }
    }
}