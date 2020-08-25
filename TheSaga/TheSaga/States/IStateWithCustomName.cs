namespace TheSaga.States
{
    public interface IStateWithCustomName : IState
    {
        string Name { get; }
    }
}