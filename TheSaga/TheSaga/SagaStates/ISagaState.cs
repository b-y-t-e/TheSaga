using System;

namespace TheSaga.SagaStates
{
    public interface ISagaState
    {
        public Guid CorrelationID { get; set; }
        public string CurrentState { get; set; }
        public string CurrentStep { get; set; }
        public bool IsCompensating { get; set; }
        public string CurrentError { get; set; }
    }
}