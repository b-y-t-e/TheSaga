using System;
using TheSaga.Execution.Actions;

namespace TheSaga.InternalMessages
{
    internal class SagaExecutionStartMessage : IInternalMessage
    {
        public SagaExecutionStartMessage(Type sagaStateType, SagaID sagaID)
        {
            SagaStateType = sagaStateType;
            SagaID = sagaID;
        }

        /// <summary>
        /// Correlation ID
        /// </summary>
        public SagaID SagaID { get; }
        public Type SagaStateType { get; }
    }
}