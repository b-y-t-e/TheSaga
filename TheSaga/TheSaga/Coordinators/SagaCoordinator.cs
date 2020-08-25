using System;
using System.Threading.Tasks;
using TheSaga.Executors;
using TheSaga.Interfaces;
using TheSaga.Models;
using TheSaga.Registrator;
using TheSaga.Persistance;
using TheSaga.States;
using TheSaga.Exceptions;

namespace TheSaga.Coordinators
{
    public class SagaCoordinator : ISagaCoordinator
    {
        ISagaRegistrator sagaRegistrator;

        public SagaCoordinator(ISagaRegistrator sagaRegistrator)
        {
            this.sagaRegistrator = sagaRegistrator;
            // this.sagaExecutor = sagaExecutor;
        }

        public async Task<ISagaState> Send(IEvent @event)
        {
            Type eventType = @event.GetType();

            ISagaModel model = sagaRegistrator.FindModelForEventType(eventType);
            if (model == null)
                throw new SagaEventNotRegisteredException(eventType);

            ISagaExecutor sagaExecutor = sagaRegistrator.
                FindExecutorForStateType(model.SagaStateType);

            return await sagaExecutor.
                Handle(@event.CorrelationID, model, @event);
        }

        public Task<ISagaState> Publish(IEvent @event)
        {
            throw new NotImplementedException();
        }

    }
}