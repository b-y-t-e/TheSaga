namespace TheSaga.States
{
    public interface IStateWithCustomName : ISagaState
    {
        string Name { get; }
    }
}