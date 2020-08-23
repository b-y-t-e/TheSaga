using System;
using System.Threading.Tasks;

namespace TheSaga
{
    public class SagaCoordinator : ISagaCoordinator
    {

        ISagaSeeker sagaSeeker { get; }

        public SagaCoordinator(ISagaSeeker sagaSeeker)
        {
            this.sagaSeeker = sagaSeeker;
        }


        public async Task Execute(IEvent @event)
        {
            ISagaInstance sagaInstance = await sagaSeeker.Seek(@event.CorrelationID);
            if (sagaInstance == null)
                throw new Exception("saga not found");
        }


        Task ISagaCoordinator.Send(IEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}