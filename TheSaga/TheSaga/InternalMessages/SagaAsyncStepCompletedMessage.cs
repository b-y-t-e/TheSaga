using System;

namespace TheSaga.InternalMessages
{
    internal class SagaAsyncStepCompletedMessage : IInternalMessage
    {
        public SagaAsyncStepCompletedMessage(Type sagaStateType, Guid sagaID, string currentState, string currentStep, bool isCompensating)
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
        public Guid SagaID { get; }
        public string CurrentState { get; }
        public string CurrentStep { get; }
        public bool IsCompensating { get; }
        public Type SagaStateType { get; }
    }
}