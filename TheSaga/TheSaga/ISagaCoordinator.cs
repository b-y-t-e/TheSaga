using System;

namespace TheSaga
{
    public interface ISagaCoordinator
    {
        void Send(IEvent @event);
    }
}