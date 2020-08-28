using TheSaga.MessageBus;
using TheSaga.SagaModels;
using TheSaga.ValueObjects;

namespace TheSaga.Messages
{
    internal class AsyncStepCompletedMessage : IInternalMessage
    {
        public AsyncStepCompletedMessage(SagaID sagaID, string currentState, 
            string currentStep, bool isCompensating, ISagaModel model)
        {
            SagaID = sagaID;
            CurrentState = currentState;
            CurrentStep = currentStep;
            IsCompensating = isCompensating;
            Model = model;
        }

        /// <summary>
        /// Correlation ID
        /// </summary>
        public SagaID SagaID { get; }
        public ISagaModel Model { get; }
        public string CurrentState { get; }
        public string CurrentStep { get; }
        public bool IsCompensating { get; }
    }
}