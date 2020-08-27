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
            Guid id = @event.ID;
            Boolean newSagaCreated = false;

            ISagaModel model = sagaRegistrator.FindModelForEventType(eventType);
            if (model == null)
                throw new SagaEventNotRegisteredException(eventType);

            Guid? newID = await CreateNewSagaIfRequired(model, id, eventType);
            if (newID != null)
            {
                id = newID.Value;
                newSagaCreated = true;
            }

            try
            {
                SendInternalMessages(id, model);

                ISagaExecutor sagaExecutor = sagaRegistrator.
                    FindExecutorForStateType(model.SagaStateType);

                return await sagaExecutor.
                    Handle(id, @event, IsExecutionAsync.False());
            }
            catch
            {
                if (newSagaCreated)
                {
                    await sagaPersistance.
                        Remove(id);
                }

                throw;
            }
        }

        public async Task WaitForState<TState>(Guid id, SagaWaitOptions waitOptions = null)
            where TState : IState, new()
        {
            if (waitOptions == null)
                waitOptions = new SagaWaitOptions();

            try
            {
                bool stateChanged = false;

                internalMessageBus.Subscribe<SagaStateChangedMessage>(this, (mesage) =>
                {
                    if (mesage.SagaID == id &&
                        mesage.CurrentState == new TState().GetStateName())
                    {
                        stateChanged = true;
                    }
                    return Task.CompletedTask;
                });

                ISaga saga = await sagaPersistance.
                    Get(id);

                if (saga == null)
                    throw new SagaInstanceNotFoundException(id);

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

        private async Task<Guid> CreateNewSaga(ISagaModel model, Guid id)
        {
            if (id == Guid.Empty)
                id = Guid.NewGuid();

            ISagaData data = (ISagaData)Activator.CreateInstance(model.SagaStateType);
            data.ID = id;

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

            return id;
        }

        private async Task<Guid?> CreateNewSagaIfRequired(ISagaModel model, Guid id, Type eventType)
        {
            if (eventType != null)
            {
                bool isStartEvent = model.IsStartEvent(eventType);
                if (isStartEvent)
                {
                    id = await CreateNewSaga(model, id);
                    return id;
                }
            }

            return null;
        }

        private void SendInternalMessages(Guid id, ISagaModel model)
        {
            internalMessageBus.Publish(
                new SagaProcessingStartMessage(model.SagaStateType, id));
        }
    }
}