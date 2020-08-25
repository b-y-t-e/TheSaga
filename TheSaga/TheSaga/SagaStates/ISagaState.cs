using System;
using System.Collections.Generic;

namespace TheSaga.SagaStates
{
    public interface ISagaState
    {
        public Guid CorrelationID { get; set; }
        public string CurrentState { get; set; }
        public string CurrentStep { get; set; }
        public bool IsCompensating { get; set; }
        public Exception CurrentError { get; set; }
        public IList<SagaStepLog> History { get; set; }
    }

    public class SagaStepLog
    {
        public bool HasSucceeded { get; set; }
        public Exception Error { get; set; }
        public string StateName { get; set; }
        public string StepName { get; set; }
        public DateTime Created { get; set; }
        public bool HasFinished { get; set; }
        public string NextStepName { get; set; }
        public bool IsCompensating { get; set; }
        public bool Async { get; set; }
    }
}