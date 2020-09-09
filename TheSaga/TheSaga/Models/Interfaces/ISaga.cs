namespace TheSaga.Models
{
    public interface ISaga
    {
        ISagaData Data { get; }
        SagaInfo Info { get; }
        SagaExecutionState State { get; }
    }
}