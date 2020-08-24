using System;
using System.Threading.Tasks;
using TheSaga.Instances;
using TheSaga.Interfaces;
using TheSaga.Models;
using TheSaga.Registrator;
using TheSaga.Seekers;
using TheSaga.States;

namespace TheSaga.Coordinators
{
    public class SagaCoordinator : ISagaCoordinator
    {
        private readonly ISagaRegistrator sagaRegistrator;

        ISagaSeeker sagaSeeker { get; }

        public SagaCoordinator(ISagaSeeker sagaSeeker, ISagaRegistrator sagaRegistrator)
        {
            this.sagaSeeker = sagaSeeker;
            this.sagaRegistrator = sagaRegistrator;
        }


        public async Task<ISagaState> Execute(IEvent @event)
        {
            ISagaModel model = sagaRegistrator.FindModel(@event);
            if (model == null)
                throw new Exception($"Event of type {@event.GetType().Name} is not registered");

            bool isStartEvent = model.IsStartEvent(@event.GetType());
            if (isStartEvent)
            {
                return await StartSaga(model);
            }

            ISagaInstance sagaInstance = await sagaSeeker.Seek(@event.CorrelationID);
            if (sagaInstance == null)
                throw new Exception("saga not found");

            await sagaInstance.Push(@event);

            return sagaInstance.State;
        }

        public Task<ISagaState> Send(IEvent @event)
        {
            throw new NotImplementedException();
        }

        private async Task<ISagaState> StartSaga(ISagaModel model)
        {
            throw new NotImplementedException();
        }
    }
}