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
        public bool? HasSucceeded { get; internal set; }
        public Exception Error { get; internal set; }
        public string State { get; internal set; }
        public string Step { get; internal set; }
        public DateTime Created { get; internal set; }
        public bool HasFinished { get; internal set; }
        public string NextStep { get; internal set; }
    }
}