namespace TheSaga.Models.Interfaces
{
    public interface ISaga
    {
        ISagaData Data { get; }
        SagaExecutionInfo ExecutionInfo { get; }
        SagaExecutionState ExecutionState { get; }
        SagaExecutionValues ExecutionValues { get; }
    }
}
