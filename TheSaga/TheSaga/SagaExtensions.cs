using System;

namespace TheSaga
{
    public static class SagaExtensions
    {
        public static ISaga<TState> Start<TState>(
            this ISaga<TState> saga,
            IEvent @event) where TState : ISagaState
        {
            return saga;
        }

        public static ISaga<TState> During<TState>(
            this ISaga<TState> saga,
            IState state) where TState : ISagaState
        {
            return saga;
        }

        public static ISaga<TState> When<TState>(
            this ISaga<TState> saga,
            IEvent @event)
            where TState : ISagaState
        {
            return saga;
        }

        public static ISaga<TState> Then<TState>(
            this ISaga<TState> saga,
            Type activityType)
            where TState : ISagaState
            // where TActivity : ISagaActivity 
        {
            return saga;
        }

        public static ISaga<TState> Then<TState>(
            this ISaga<TState> saga,
            Type activityType,
            Type compensateType)
            where TState : ISagaState
            // where TActivity : ISagaActivity 
        {
            return saga;
        }


        public static ISaga<TState> After<TState>(
            this ISaga<TState> saga,
            TimeSpan time) where TState : ISagaState
        {
            return saga;
        }

        public static ISaga<TState> Then<TState>(
            this ISaga<TState> saga,
            ThenFunction action) where TState : ISagaState
        {
            action(null);
            return saga;
        }

        public static ISaga<TState> TransitionTo<TState>(
            this ISaga<TState> saga,
            IState state) where TState : ISagaState
        {
            return saga;
        }
    }

    public delegate void ThenFunction(IContext context);
    public interface IContext
    {
    }
}