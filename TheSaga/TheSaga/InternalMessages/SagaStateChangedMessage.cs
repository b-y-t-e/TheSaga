using System;
using TheSaga.Execution.Actions;

namespace TheSaga.InternalMessages
{
    internal class SagaStateChangedMessage : IInternalMessage
    {
        public SagaStateChangedMessage(Type sagaStateType, SagaID sagaID, string currentState, string currentStep, bool isCompensating)
        {
            SagaStateType = sagaStateType;
            SagaID = sagaID;
            CurrentState = currentState;
            CurrentStep = currentStep;
            IsCompensating = isCompensating;
        }

        /// <summary>
        /// Correlation ID
        /// </summary>
        public SagaID SagaID { get; }
        public string CurrentState { get; }
        public string CurrentStep { get; }
        public bool IsCompensating { get; }
        public Type SagaStateType { get; }
    }
}