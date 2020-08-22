using System;

namespace TheSaga
{
    public static class SagaExtensions
    {
        public static ISaga Start(
            this ISaga saga,
            IEvent @event)
        {
            return saga;
        }

        public static ISaga During(
            this ISaga saga,
            IState state)
        {
            return saga;
        }

        public static ISaga When(
            this ISaga saga,
            IEvent @event)
        {
            return saga;
        }

        public static ISaga After(
            this ISaga saga,
            TimeSpan time)
        {
            return saga;
        }

        public static ISaga Then(
            this ISaga saga,
            ThenFunction action)
        {
            action(null);
            return saga;
        }

        public static ISaga TransitionTo(
            this ISaga saga,
            IState state)
        {
            return saga;
        }
    }

    public delegate void ThenFunction(IContext context);
    public interface IContext
    {
    }
}