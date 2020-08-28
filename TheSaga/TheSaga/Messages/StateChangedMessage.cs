using TheSaga.Messages.MessageBus;
using TheSaga.ValueObjects;

namespace TheSaga.Messages
{
    internal class StateChangedMessage : IInternalMessage
    {
        public StateChangedMessage(SagaID sagaID, string currentState, string currentStep, bool isCompensating)
        {
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
    }
}