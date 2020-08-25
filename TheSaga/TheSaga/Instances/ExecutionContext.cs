using TheSaga.States;

namespace TheSaga.Builders
{
    public class ExecutionContext<TState> : IInstanceContext<TState> where TState : ISagaState
    {
        public TState State { get; set; }
    }
}