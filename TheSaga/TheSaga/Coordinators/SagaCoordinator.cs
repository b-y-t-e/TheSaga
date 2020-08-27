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
            SagaID sagaId = SagaID.From(@event.ID);
            Boolean newSagaCreated = false;

            ISagaModel model = sagaRegistrator.FindModelForEventType(eventType);
            if (model == null)
                throw new SagaEventNotRegisteredException(eventType);

            SagaID newID = await CreateNewSagaIfRequired(model, sagaId, eventType);
            if (newID != SagaID.Empty())
            {
                sagaId = newID;
                newSagaCreated = true;
            }

            try
            {
                await PrepareExecutionID(sagaId);

                SendInternalMessages(sagaId, model);

                ISagaExecutor sagaExecutor = sagaRegistrator.
                    FindExecutorForStateType(model.SagaStateType);

                return await sagaExecutor.
                    Handle(sagaId, @event, IsExecutionAsync.False());
            }
            catch
            {
                if (newSagaCreated)
                {
                    await sagaPersistance.
                        Remove(sagaId);
                }

                throw;
            }
        }

        private async Task PrepareExecutionID(SagaID id)
        {
            ISaga saga = await sagaPersistance.
                Get(id);

            saga.State.
                ExecutionID = ExecutionID.New();

            await sagaPersistance.
                Set(saga);
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

        private async Task<SagaID> CreateNewSagaIfRequired(ISagaModel model, SagaID id, Type eventType)
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

            return SagaID.Empty();
        }

        private async Task<SagaID> CreateNewSaga(ISagaModel model, SagaID id)
        {
            if (id == SagaID.Empty())
                id = SagaID.New();

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

        private void SendInternalMessages(SagaID id, ISagaModel model)
        {
            internalMessageBus.Publish(
                new SagaProcessingStartMessage(model.SagaStateType, id));
        }
    }
}