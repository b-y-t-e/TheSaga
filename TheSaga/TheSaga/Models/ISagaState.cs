namespace TheSaga.Models
{
    public interface ISagaState
    {
        SagaInfo Info { get; }

        SagaState State { get; }
    }
}