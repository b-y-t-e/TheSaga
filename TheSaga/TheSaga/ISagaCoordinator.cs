using System;
using System.Threading.Tasks;

namespace TheSaga
{
    public interface ISagaCoordinator
    {
        Task Send(IEvent @event);
        Task Execute(IEvent @event);
    }
}