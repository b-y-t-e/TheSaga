namespace TheSaga.Models
{
    public interface ISaga : ISagaState
    {
        ISagaData Data { get; }
    }
}