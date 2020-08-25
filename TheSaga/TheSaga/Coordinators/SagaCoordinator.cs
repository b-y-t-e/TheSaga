using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Execution;
using TheSaga.Models;
using TheSaga.Options;
using TheSaga.Persistance;
using TheSaga.Registrator;
using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Coordinators
{
    public class SagaCoordinator : ISagaCoordinator
    {
        private ISagaRegistrator sagaRegistrator;
        private ISagaPersistance sagaPersistance;

        public SagaCoordinator(ISagaRegistrator sagaRegistrator, ISagaPersistance sagaPersistance)
        {
            this.sagaRegistrator = sagaRegistrator;
            this.sagaPersistance = sagaPersistance;
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

        public async Task WaitForEvent<TSagaEvent>(Guid correlationID, SagaWaitOptions waitOptions = null) 
            where TSagaEvent : IEvent
        {
            ISagaState state = await sagaPersistance.
                Get(correlationID);

            if (state == null)
                throw new SagaInstanceNotFoundException(correlationID);

        }

        public async Task WaitForState<TState>(Guid correlationID, SagaWaitOptions waitOptions = null)
            where TState : IState, new()
        {
            if (waitOptions == null)
                waitOptions = new SagaWaitOptions();

            ISagaState state = await sagaPersistance.
                Get(correlationID);

            if (state == null)
                throw new SagaInstanceNotFoundException(correlationID);

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (true)
            {
                await Task.Delay(250);
                
                state = await sagaPersistance.
                    Get(correlationID);
                
                if (state.CurrentState == new TState().GetStateName())
                    break;

                if (stopwatch.Elapsed >= waitOptions.Timeout)
                    throw new TimeoutException();
            }
        }
    }
}