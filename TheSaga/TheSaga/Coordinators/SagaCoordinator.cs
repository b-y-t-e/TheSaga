﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TheSaga.Coordinators.AsyncHandlers;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Execution;
using TheSaga.InternalMessages;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Models;
using TheSaga.Options;
using TheSaga.Persistance;
using TheSaga.Registrator;
using TheSaga.SagaStates;
using TheSaga.States;
using TheSaga.Utils;

namespace TheSaga.Coordinators
{
    public class SagaCoordinator : ISagaCoordinator
    {
        private ISagaRegistrator sagaRegistrator;
        private ISagaPersistance sagaPersistance;
        private IInternalMessageBus internalMessageBus;

        public SagaCoordinator(ISagaRegistrator sagaRegistrator, ISagaPersistance sagaPersistance, IInternalMessageBus internalMessageBus)
        {
            this.sagaRegistrator = sagaRegistrator;
            this.sagaPersistance = sagaPersistance;
            this.internalMessageBus = internalMessageBus;

            new SagaProcessingMessageHandler(internalMessageBus).
                Subscribe();
        }

        public async Task<ISagaState> Send(IEvent @event)
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
                    Handle(correlationID, @event, false);
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

        private void SendInternalMessages(Guid correlationID, ISagaModel model)
        {
            internalMessageBus.Publish(
                                new SagaProcessingStartMessage(model.SagaStateType, correlationID));
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

        private async Task<Guid> CreateNewSaga(ISagaModel model, Guid correlationID)
        {
            if (correlationID == Guid.Empty)
                correlationID = Guid.NewGuid();

            ISagaState newSagaState = (ISagaState)Activator.CreateInstance(model.SagaStateType);
            newSagaState.CorrelationID = correlationID;
            newSagaState.CurrentState = new SagaStartState().GetStateName();
            newSagaState.CurrentStep = null;

            await sagaPersistance.
                Set(newSagaState);

            return correlationID;
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

                ISagaState state = await sagaPersistance.
                    Get(correlationID);

                if (state == null)
                    throw new SagaInstanceNotFoundException(correlationID);

                if (state.CurrentState == new TState().GetStateName())
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

    }
}