using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TheSaga.Coordinators.AsyncHandlers;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Execution;
using TheSaga.Execution.Actions;
using TheSaga.Execution.AsyncHandlers;
using TheSaga.InternalMessages;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Models;
using TheSaga.Options;
using TheSaga.Persistance;
using TheSaga.Providers;
using TheSaga.Registrator;
using TheSaga.SagaStates;
using TheSaga.States;

namespace TheSaga.Coordinators
{
    public class SagaCoordinator : ISagaCoordinator
    {
        private IInternalMessageBus internalMessageBus;
        private ISagaPersistance sagaPersistance;
        private ISagaRegistrator sagaRegistrator;
        private IDateTimeProvider dateTimeProvider;
        public SagaCoordinator(ISagaRegistrator sagaRegistrator, ISagaPersistance sagaPersistance, IInternalMessageBus internalMessageBus, IDateTimeProvider dateTimeProvider)
        {
            this.sagaRegistrator = sagaRegistrator;
            this.sagaPersistance = sagaPersistance;
            this.internalMessageBus = internalMessageBus;
            this.dateTimeProvider = dateTimeProvider;

            new SagaProcessingMessageHandler(internalMessageBus).
                Subscribe();
        }

        public async Task<ISaga> Send(IEvent @event)
        {
            Type eventType = @event.GetType();
            Guid correlationID = @event.CorrelationID;
            Boolean newSagaCreated = false;

            ISagaModel model = sagaRegistrator.FindModelForEventType(eventType);
            if (model == null)
                throw new SagaEventNotRegisteredException(eventType);

            Guid? newCorrelationID = await CreateNewSagaIfRequired(model, correlationID, eventType);
            if (newCorrelationID != null)
            {
                correlationID = newCorrelationID.Value;
                newSagaCreated = true;
            }

            try
            {
                SendInternalMessages(correlationID, model);

                ISagaExecutor sagaExecutor = sagaRegistrator.
                    FindExecutorForStateType(model.SagaStateType);

                return await sagaExecutor.
                    Handle(correlationID, @event, IsExecutionAsync.False());
            }
            catch
            {
                if (newSagaCreated)
                {
                    await sagaPersistance.
                        Remove(correlationID);
                }

                throw;
            }
        }

        public async Task WaitForState<TState>(Guid correlationID, SagaWaitOptions waitOptions = null)
            where TState : IState, new()
        {
            if (waitOptions == null)
                waitOptions = new SagaWaitOptions();

            try
            {
                bool stateChanged = false;

                internalMessageBus.Subscribe<SagaStateChangedMessage>(this, (mesage) =>
                {
                    if (mesage.CorrelationID == correlationID &&
                        mesage.CurrentState == new TState().GetStateName())
                    {
                        stateChanged = true;
                    }
                    return Task.CompletedTask;
                });

                ISaga saga = await sagaPersistance.
                    Get(correlationID);

                if (saga == null)
                    throw new SagaInstanceNotFoundException(correlationID);

                if (saga.State.CurrentState == new TState().GetStateName())
                    return;

                Stopwatch stopwatch = Stopwatch.StartNew();
                while (!stateChanged)
                {
                    await Task.Delay(250);
                    if (stopwatch.Elapsed >= waitOptions.Timeout)
                        throw new TimeoutException();
                }
            }
            finally
            {
                internalMessageBus.Unsubscribe<SagaStateChangedMessage>(this);
            }
        }

        private async Task<Guid> CreateNewSaga(ISagaModel model, Guid correlationID)
        {
            if (correlationID == Guid.Empty)
                correlationID = Guid.NewGuid();

            ISagaData data = (ISagaData)Activator.CreateInstance(model.SagaStateType);
            data.CorrelationID = correlationID;

            ISaga saga = new Saga()
            {
                Data = data,
                Info = new SagaInfo()
                {
                    Created = dateTimeProvider.Now,
                    Modified = dateTimeProvider.Now
                },
                State = new SagaState()
                {
                    CurrentState = new SagaStartState().GetStateName(),
                    CurrentStep = null
                }
            };

            await sagaPersistance.
                Set(saga);

            return correlationID;
        }

        private async Task<Guid?> CreateNewSagaIfRequired(ISagaModel model, Guid correlationID, Type eventType)
        {
            if (eventType != null)
            {
                bool isStartEvent = model.IsStartEvent(eventType);
                if (isStartEvent)
                {
                    correlationID = await CreateNewSaga(model, correlationID);
                    return correlationID;
                }
            }

            return null;
        }

        private void SendInternalMessages(Guid correlationID, ISagaModel model)
        {
            internalMessageBus.Publish(
                new SagaProcessingStartMessage(model.SagaStateType, correlationID));
        }
    }
}