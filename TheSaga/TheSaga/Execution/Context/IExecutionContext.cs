using TheSaga.SagaStates;

namespace TheSaga.Execution.Context
{
    public interface IExecutionContext<TState> : IExecutionContext where TState : ISagaState
    {
        TState State { get; }
    }

    public interface IExecutionContext
    {
    }
}