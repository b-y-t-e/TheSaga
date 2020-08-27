﻿using System;
using TheSaga.Execution.Actions;

namespace TheSaga.InternalMessages
{
    internal class SagaProcessingStartMessage : IInternalMessage
    {
        public SagaProcessingStartMessage(Type sagaStateType, SagaID sagaID)
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