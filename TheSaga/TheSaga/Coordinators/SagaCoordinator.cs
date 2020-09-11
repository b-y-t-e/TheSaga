using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Commands;
using TheSaga.Commands.Handlers;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.MessageBus;
using TheSaga.MessageBus.Interfaces;
using TheSaga.Messages;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.Observables.Registrator;
using TheSaga.Options;
using TheSaga.Persistance;
using TheSaga.Providers;
using TheSaga.Providers.Interfaces;
using TheSaga.Registrator;
using TheSaga.Registrator.Interfaces;
using TheSaga.States;
using TheSaga.ValueObjects;

namespace TheSaga.Coordinators
{
    public class SagaCoordinator : ISagaCoordinator
    {
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IMessageBus messageBus;
        private readonly ISagaPersistance sagaPersistance;
        private readonly ISagaRegistrator sagaRegistrator;
        private readonly IServiceProvider serviceProvider;

        public SagaCoordinator(ISagaRegistrator sagaRegistrator, ISagaPersistance sagaPersistance,
            IMessageBus messageBus, IDateTimeProvider dateTimeProvider,
            IServiceProvider serviceProvider)
        {
            this.sagaRegistrator = sagaRegistrator;
            this.sagaPersistance = sagaPersistance;
            this.messageBus = messageBus;
            this.dateTimeProvider = dateTimeProvider;
            this.serviceProvider = serviceProvider;
        }

        public async Task ResumeAll()
        {
            IList<Guid> ids = await sagaPersistance.GetUnfinished();

            List<string> invalidModels = new List<string>();
            foreach (Guid id in ids)
            {
                ISaga saga = await sagaPersistance.Get(id);
                ISagaModel model = sagaRegistrator.FindModelByName(saga.ExecutionInfo.ModelName);

                if (model == null)
                    invalidModels.Add(saga.ExecutionInfo.ModelName);
            }

            if (invalidModels.Count > 0)
                throw new Exception($"Saga models {string.Join(", ", invalidModels.Distinct().ToArray())} not found");

            foreach (Guid id in ids)
            {
                ISaga saga = await sagaPersistance.Get(id);

                ISagaModel model = sagaRegistrator.FindModelByName(saga.ExecutionInfo.ModelName);

                await ExecuteSaga(
                    new EmptyEvent(),
                    model,
                    saga,
                    saga.Data.ID,
                    null,
                    true);
            }
        }
        public async Task Resume(Guid id)
        {
            List<string> invalidModels = new List<string>();

            ISaga saga = await sagaPersistance.Get(id);
            ISagaModel model = sagaRegistrator.FindModelByName(saga.ExecutionInfo.ModelName);

            if (model == null)
                invalidModels.Add(saga.ExecutionInfo.ModelName);

            if (invalidModels.Count > 0)
                throw new Exception($"Saga models {string.Join(", ", invalidModels.Distinct().ToArray())} not found");

            await ExecuteSaga(
                new EmptyEvent(),
                model,
                saga,
                saga.Data.ID,
                null,
                true);
        }

        public Task<ISaga> Publish(
            ISagaEvent @event)
        {
            return Publish(@event, null);
        }

        public async Task<ISaga> Publish(ISagaEvent @event, IDictionary<string, object> executionValues)
        {
            Type eventType = @event.GetType();
            SagaID sagaId = SagaID.From(@event.ID);

            ISagaModel model = sagaRegistrator.
                FindModelForEventType(eventType);

            if (model == null)
                throw new SagaEventNotRegisteredException(eventType);

            ISaga newSaga = await CreateNewSagaIfRequired(model, sagaId, eventType);

            try
            {

                return await ExecuteSaga(
                    @event,
                    model,
                    newSaga,
                    SagaID.From(newSaga?.Data?.ID ?? sagaId.Value),
                    executionValues,
                    false);
            }
            catch
            {
                if (newSaga != null)
                    await sagaPersistance.Remove(newSaga.Data.ID);

                throw;
            }
        }

        public async Task WaitForIdle(
            Guid id, SagaWaitOptions waitOptions = null)
        {
            if (waitOptions == null)
                waitOptions = new SagaWaitOptions();

            try
            {
                bool isSagaInIdleState = false;

                messageBus.Subscribe<ExecutionEndMessage>(this, mesage =>
                {
                    if (mesage.Saga.Data.ID == id &&
                        mesage.Saga.IsIdle())
                        isSagaInIdleState = true;

                    return Task.CompletedTask;
                });

                ISaga saga = await sagaPersistance.Get(id);

                if (saga == null)
                    throw new SagaInstanceNotFoundException(id);

                if (saga.IsIdle())
                    return;

                Stopwatch stopwatch = Stopwatch.StartNew();
                while (!isSagaInIdleState)
                {
                    await Task.Delay(250);
                    if (stopwatch.Elapsed >= waitOptions.Timeout)
                        throw new TimeoutException();
                }
            }
            finally
            {
                messageBus.Unsubscribe<ExecutionEndMessage>(this);
            }
        }

        private async Task<ISaga> ExecuteSaga(
            ISagaEvent @event, ISagaModel model,
            ISaga saga,
            Guid sagaID,
            IDictionary<string, object> executionValues,
            bool resume)
        {
            bool sagaStarted = false;

            try
            {
                serviceProvider.
                    GetRequiredService<ObservableRegistrator>().
                    Initialize();

                await messageBus.
                    Publish(new ExecutionStartMessage(SagaID.From(sagaID), model));

                sagaStarted = true;

                if (saga == null)
                    saga = await sagaPersistance.Get(sagaID);

                if (saga == null)
                    throw new SagaInstanceNotFoundException();

                if (saga.IsIdle())
                {
                    saga.ExecutionState.CurrentError = null;
                    saga.ExecutionState.ExecutionID = ExecutionID.New();
                    if (model.HistoryPolicy == ESagaHistoryPolicy.StoreOnlyCurrentStep)
                        saga.ExecutionState.History.Clear();
                }
                else
                {
                    if (!resume)
                        throw new SagaNeedToBeResumedException(saga.Data.ID);
                }

                ExecuteActionCommandHandler handler = serviceProvider.
                    GetRequiredService<ExecuteActionCommandHandler>();

                saga.ExecutionValues.
                    Set(executionValues);

                saga.ExecutionState.
                    CurrentEvent = @event ?? new EmptyEvent();

                return await handler.Handle(new ExecuteActionCommand
                {
                    Async = AsyncExecution.False(),
                    Saga = saga,
                    Model = model
                });
            }
            catch
            {
                if (sagaStarted)
                    await messageBus.Publish(
                        new ExecutionEndMessage(saga));

                throw;
            }
        }

        private async Task<ISaga> CreateNewSagaIfRequired(ISagaModel model, SagaID id, Type eventType)
        {
            ISaga saga = null;

            if (eventType != null)
            {
                bool isStartEvent = model.Actions.IsStartEvent(eventType);

                if (isStartEvent)
                    saga = await CreateNewSaga(model, id);
            }

            return saga;
        }

        private async Task<ISaga> CreateNewSaga(ISagaModel model, SagaID id)
        {
            if (id == SagaID.Empty())
                id = SagaID.New();

            ISagaData data = (ISagaData)Activator.CreateInstance(model.SagaStateType);
            data.ID = id;

            ISaga saga = new Saga
            {
                Data = data,
                ExecutionInfo = new SagaExecutionInfo
                {
                    ModelName = model.Name,
                    Created = dateTimeProvider.Now,
                    Modified = dateTimeProvider.Now
                },
                ExecutionState = new SagaExecutionState
                {
                    CurrentState = new SagaStartState().GetStateName(),
                    CurrentStep = null
                }
            };

            return saga;
        }

    }
}
