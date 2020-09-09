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
using TheSaga.Messages;
using TheSaga.Models;
using TheSaga.Observables.Registrator;
using TheSaga.Options;
using TheSaga.Persistance;
using TheSaga.Providers;
using TheSaga.Registrator;
using TheSaga.SagaModels;
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
                ISagaModel model = sagaRegistrator.FindModelByName(saga.Info.ModelName);

                if (model == null)
                    invalidModels.Add(saga.Info.ModelName);
            }

            if (invalidModels.Count > 0)
                throw new Exception($"Saga models {string.Join(", ", invalidModels.Distinct().ToArray())} not found");

            foreach (Guid id in ids)
            {
                ISaga saga = await sagaPersistance.Get(id);

                ISagaModel model = sagaRegistrator.FindModelByName(saga.Info.ModelName);

                await ExecuteSaga(
                    new EmptyEvent(),
                    model,
                    saga);
            }
        }
        public async Task Resume(Guid id)
        {
            List<string> invalidModels = new List<string>();

            ISaga saga = await sagaPersistance.Get(id);
            ISagaModel model = sagaRegistrator.FindModelByName(saga.Info.ModelName);

            if (model == null)
                invalidModels.Add(saga.Info.ModelName);

            if (invalidModels.Count > 0)
                throw new Exception($"Saga models {string.Join(", ", invalidModels.Distinct().ToArray())} not found");

            await ExecuteSaga(
                new EmptyEvent(),
                model,
                saga);
        }

        public async Task<ISaga> Publish(
            ISagaEvent @event)
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
                ISaga saga =
                    newSaga ??
                    await sagaPersistance.Get(sagaId);

                return await ExecuteSaga(
                    @event,
                    model,
                    saga);
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
            ISagaEvent @event, ISagaModel model, ISaga saga)
        {
            try
            {
                if (saga == null)
                    throw new SagaInstanceNotFoundException();

                serviceProvider.
                    GetRequiredService<ObservableRegistrator>().
                    Initialize();

                await messageBus.
                    Publish(new ExecutionStartMessage(saga));

                ExecuteActionCommandHandler handler = serviceProvider.
                    GetRequiredService<ExecuteActionCommandHandler>();

                return await handler.Handle(new ExecuteActionCommand
                {
                    Async = AsyncExecution.False(),
                    Event = @event,
                    Saga = saga,
                    Model = model
                });
            }
            catch
            {
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
                Info = new SagaInfo
                {
                    ModelName = model.Name,
                    Created = dateTimeProvider.Now,
                    Modified = dateTimeProvider.Now
                },
                State = new SagaState
                {
                    CurrentState = new SagaStartState().GetStateName(),
                    CurrentStep = null
                }
            };

            return saga;
        }

    }
}