using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Execution.Context
{
    public class ExecutionContext<TState> : IExecutionContext<TState> where TState : ISagaState
    {
        public TState State { get; set; }
    }
}