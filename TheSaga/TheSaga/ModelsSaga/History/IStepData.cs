namespace TheSaga.SagaModels.History
{
    public interface IStepData
    {
        public StepExecutionData ExecutionData { get; }
        public StepExecutionData CompensationData { get; }
        public StepExecutionData ResumeData { get;  }
    }
}