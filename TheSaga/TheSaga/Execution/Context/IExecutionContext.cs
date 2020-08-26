using TheSaga.SagaStates;

namespace TheSaga.Execution.Context
{
    public interface IExecutionContext<TState> : IExecutionContext where TState : ISagaData
    {
        TState State { get; }
    }

    public interface IExecutionContext
    {
    }
}