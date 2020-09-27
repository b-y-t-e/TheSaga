using System;
using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Models.History.Retry;

namespace TheSaga.Models.History
{
    public class StepData : IStepData
    {
        public Guid ID { get; set; }
        public Guid ExecutionID { get; set; }
        public string StateName { get; set; }
        public string StepName { get; set; }
        public string EndStateName { get; set; }
        public string NextStepName { get; set; }
        public bool AsyncExecution { get; set; }
        public bool AsyncStep { get; set; }
        public ISagaEvent Event { get; set; }
        public StepExecutionValues ExecutionValues { get; set; }
        public StepExecutionData ExecutionData { get; set; }
        public StepExecutionData CompensationData { get; set; }
        public StepExecutionData ResumeData { get; set; }
        public IStepRetryData RetryData { get; set; }

        public StepData()
        {
            ID = Guid.NewGuid();
        }
    }
}
