using System;
using System.Net.NetworkInformation;
using TheSaga.Model;

namespace TheSaga
{
    public interface ISagaBuilder<TSagaType, TState>
            where TSagaType : ISaga<TState>
            where TState : ISagaState
    {

        ISaga<TState> Start(

            IEvent @event);

        ISaga<TState> During(

            IState state);

        ISaga<TState> When(

            IEvent @event)
            ;

        ISaga<TState> Then(
            Type activityType)
            ;

        ISaga<TState> Then(

            Type activityType,
            Type compensateType)
            ;


        ISaga<TState> After(

            TimeSpan time);

        ISaga<TState> Then(

            ThenFunction action);

        ISaga<TState> TransitionTo(

            IState state);
        SagaModel Build();
    }
}