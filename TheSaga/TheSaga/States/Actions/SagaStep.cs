using System;
using System.Threading.Tasks;
using TheSaga.Builders;

namespace TheSaga.States.Actions
{
    public class SagaStep<TSagaState> where TSagaState : ISagaState
    {
        public ThenFunction<TSagaState> Action;

        public Type Activity;
    }

    public delegate Task ThenFunction<TState>(IInstanceContext<TState> context) where TState : ISagaState;
}