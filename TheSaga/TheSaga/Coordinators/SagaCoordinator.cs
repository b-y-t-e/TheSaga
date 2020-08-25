using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Execution;
using TheSaga.Models;
using TheSaga.Registrator;
using TheSaga.SagaStates;

namespace TheSaga.Coordinators
{
    public class SagaCoordinator : ISagaCoordinator
    {
        private ISagaRegistrator sagaRegistrator;

        public SagaCoordinator(ISagaRegistrator sagaRegistrator)
        {
            this.sagaRegistrator = sagaRegistrator;
            // this.sagaExecutor = sagaExecutor;
        }

        public Task<ISagaState> Publish(IEvent @event)
        {
            throw new NotImplementedException();
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
    }
}