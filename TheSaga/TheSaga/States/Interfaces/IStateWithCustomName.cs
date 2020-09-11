namespace TheSaga.States.Interfaces
{
    public interface IStateWithCustomName : ISagaState
    {
        string Name { get; }
    }
}
