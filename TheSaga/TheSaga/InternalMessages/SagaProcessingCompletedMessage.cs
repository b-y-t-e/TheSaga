using System;

namespace TheSaga.InternalMessages
{
    internal class SagaProcessingCompletedMessage : IInternalMessage
    {
        public SagaProcessingCompletedMessage(Type sagaStateType, Guid sagaID)
        {
            SagaStateType = sagaStateType;
            SagaID = sagaID;
        }

        /// <summary>
        /// Correlation ID
        /// </summary>
        public Guid SagaID { get; }
        public Type SagaStateType { get; }
    }
}