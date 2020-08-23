using System;

namespace TheSaga
{
    public interface ISagaCoordinator
    {
        void Send(IEvent @event);
        void Execute(IEvent @event);
    }
}