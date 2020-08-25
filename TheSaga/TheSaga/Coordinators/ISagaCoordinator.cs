using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Options;
using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Coordinators
{
    public interface ISagaCoordinator
    {
        Task<ISagaState> Publish(IEvent @event);
        Task<ISagaState> Send(IEvent @event);
        Task WaitForState<TState>(Guid correlationID, SagaWaitOptions waitOptions = null)
            where TState : IState;
        Task WaitForEvent<TEvent>(Guid correlationID, SagaWaitOptions waitOptions = null)
            where TEvent : IEvent;
    }
}