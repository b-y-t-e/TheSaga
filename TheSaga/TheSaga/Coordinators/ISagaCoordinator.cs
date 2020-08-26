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
        Task<ISagaData> Send(IEvent @event);

        Task WaitForState<TState>(Guid correlationID, SagaWaitOptions waitOptions = null)
            where TState : IState, new();
    }
}