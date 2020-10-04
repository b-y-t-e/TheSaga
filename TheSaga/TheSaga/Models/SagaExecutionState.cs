using System;
using System.Linq;
using TheSaga.Events;
using TheSaga.Models.History;
using TheSaga.ValueObjects;

namespace TheSaga.Models
{
    public class SagaExecutionState : ISagaExecutionState
    {
        public SagaExecutionState()
        {
            History = new SagaHistory();
            ExecutionID = ExecutionID.Empty();
        }

        public ISagaEvent CurrentEvent { get; set; }
        public Exception CurrentError { get; set; }
        public string CurrentState { get; set; }
        public string CurrentStep { get; set; }
        public bool IsCompensating { get; set; }
        public bool IsResuming { get; set; }
        public SagaHistory History { get; set; }
        public ExecutionID ExecutionID { get; set; }
        public AsyncExecution AsyncExecution { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBreaked { get; set; }
        public string GetExecutionState()
        {
            StepData item = History.FirstOrDefault(i => i.ExecutionID == ExecutionID);
            return item?.StateName ?? CurrentState;
        }
        public StepData CurrentStepData()
        {
            return History.GetLatestByStepName(ExecutionID, CurrentStep);
        }
    }
}
