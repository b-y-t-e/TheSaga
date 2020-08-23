using System;

namespace TheSaga
{
    public class SagaCoordinator : ISagaCoordinator
    {
        public SagaCoordinator()
        {
            
        }

        public void Execute(IEvent @event)
        {
            throw new NotImplementedException();
        }

        public void Send(IEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}