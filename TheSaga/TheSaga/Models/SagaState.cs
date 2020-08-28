using System;
using System.Linq;
using TheSaga.SagaModels.History;
using TheSaga.ValueObjects;

namespace TheSaga.Models
{
    public class SagaState
    {
        public SagaState()
        {
            History = new SagaHistory();
            ExecutionID = ExecutionID.Empty();
        }

        public Exception CurrentError { get; set; }
        public string CurrentState { get; set; }
        public string CurrentStep { get; set; }
        public bool IsCompensating { get; set; }
        public SagaHistory History { get; set; }
        public ExecutionID ExecutionID { get; set; }

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