namespace TheSaga.Models
{
    public interface ISaga
    {
        ISagaData Data { get; }
        SagaExecutionInfo Info { get; }
        SagaExecutionState State { get; }
        SagaExecutionValues Values { get; }
    }
}