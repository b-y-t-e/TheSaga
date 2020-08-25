using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Coordinators
{
    public interface ISagaCoordinator
    {
        Task<ISagaState> Send(IEvent @event);
        Task<ISagaState> Publish(IEvent @event);
    }
}