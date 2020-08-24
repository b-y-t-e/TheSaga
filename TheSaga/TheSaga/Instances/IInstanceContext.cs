using TheSaga.States;

namespace TheSaga.Builders
{
    public interface IInstanceContext<TState> where TState : ISagaState
    {
        TState Data { get; }
    }
}