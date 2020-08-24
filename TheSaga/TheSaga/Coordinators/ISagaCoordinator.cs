using System;
using System.Threading.Tasks;
using TheSaga.Interfaces;
using TheSaga.States;

namespace TheSaga.Coordinators
{
    public interface ISagaCoordinator
    {
        Task<ISagaState> Process(IEvent @event);
        Task<ISagaState> Publish(IEvent @event);
    }
}