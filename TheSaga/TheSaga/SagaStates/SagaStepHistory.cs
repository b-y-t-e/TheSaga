using System;

namespace TheSaga.SagaStates
{
    public class SagaStepHistory
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