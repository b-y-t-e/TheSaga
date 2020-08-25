using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.SagaStates;

namespace TheSaga.Coordinators
{
    public interface ISagaCoordinator
    {
        Task<ISagaState> Publish(IEvent @event);

        Task<ISagaState> Send(IEvent @event);
    }
}