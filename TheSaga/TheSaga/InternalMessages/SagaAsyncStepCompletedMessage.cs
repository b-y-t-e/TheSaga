using System;

namespace TheSaga.InternalMessages
{
    internal class SagaAsyncStepCompletedMessage : IInternalMessage
    {
        public SagaAsyncStepCompletedMessage(Type sagaStateType, Guid correlationID, string currentState, string currentStep, bool isCompensating)
        {
            SagaStateType = sagaStateType;
            CorrelationID = correlationID;
            CurrentState = currentState;
            CurrentStep = currentStep;
            IsCompensating = isCompensating;
        }

        public Guid CorrelationID { get; }
        public string CurrentState { get; }
        public string CurrentStep { get; }
        public bool IsCompensating { get; }
        public Type SagaStateType { get; }
    }
}