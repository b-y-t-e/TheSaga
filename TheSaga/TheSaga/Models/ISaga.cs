namespace TheSaga.Models
{
    public interface ISaga
    {
        ISagaData Data { get; }
        SagaInfo Info { get; }
        SagaState State { get; }
    }
}