using System;

namespace TheSaga.InternalMessages
{
    internal class SagaProcessingStartMessage : IInternalMessage
    {
        public SagaProcessingStartMessage(Type sagaStateType, Guid sagaID)
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