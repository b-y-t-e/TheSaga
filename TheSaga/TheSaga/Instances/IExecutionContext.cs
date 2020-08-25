using TheSaga.States;

namespace TheSaga.Builders
{

    public interface IInstanceContext<TState> : IExecutionContext where TState : ISagaState
    {
        TState State { get; }
    }

    public interface IExecutionContext
    {
    }
}