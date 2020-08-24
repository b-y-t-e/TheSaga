using TheSaga.States;

namespace TheSaga.Builders
{
    public class InstanceContext<TState> : IInstanceContext<TState> where TState : ISagaState
    {
        public TState State { get; set; }
    }

    public interface IInstanceContext<TState> : IInstanceContext where TState : ISagaState
    {
        TState State { get; }
    }

    public interface IInstanceContext
    {
    }
}