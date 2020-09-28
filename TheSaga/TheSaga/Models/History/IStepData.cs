namespace TheSaga.Models.History
{
    public interface IStepData
    {
        public StepExecutionData ExecutionData { get; }
        public StepExecutionData CompensationData { get; }
        public StepExecutionData ResumeData { get;  }
    }
}
