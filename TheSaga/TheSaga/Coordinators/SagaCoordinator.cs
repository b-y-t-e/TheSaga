using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IMessageBus internalMessageBus;
        private readonly ISagaPersistance sagaPersistance;
        private readonly ISagaRegistrator sagaRegistrator;
        private readonly IServiceProvider serviceProvider;

        public SagaCoordinator(ISagaRegistrator sagaRegistrator, ISagaPersistance sagaPersistance,
            IMessageBus internalMessageBus, IDateTimeProvider dateTimeProvider,
            IServiceProvider serviceProvider)
        {
            this.sagaRegistrator = sagaRegistrator;
            this.sagaPersistance = sagaPersistance;
            this.internalMessageBus = internalMessageBus;
            this.dateTimeProvider = dateTimeProvider;
            this.serviceProvider = serviceProvider;
        }

        public async Task ResumeAll()
        {
            var ids = await sagaPersistance.GetUnfinished();

            var invalidModels = new List<string>();
            foreach (var id in ids)
            {
                var saga = await sagaPersistance.Get(id);
                var model = sagaRegistrator.FindModelByName(saga.Info.ModelName);

                if (model == null)
                    invalidModels.Add(saga.Info.ModelName);
            }

            if (invalidModels.Count > 0)
                throw new Exception($"Saga models {string.Join(", ", invalidModels.Distinct().ToArray())} not found");

            foreach (var id in ids)
            {
                var saga = await sagaPersistance.Get(id);

                var model = sagaRegistrator.FindModelByName(saga.Info.ModelName);

                await ExecuteSaga(
                    new EmptyEvent(),
                    model,
                    saga,
                    SagaID.From(id));
            }
        }

        public async Task<ISaga> Publish(IEvent @event)
        {
            var eventType = @event.GetType();
            var sagaId = SagaID.From(@event.ID);

            var model = sagaRegistrator.FindModelForEventType(eventType);
            if (model == null)
                throw new SagaEventNotRegisteredException(eventType);

            var newSaga = await CreateNewSagaIfRequired(model, sagaId, eventType);

            try
            {
                var saga =
                    newSaga ??
                    await sagaPersistance.Get(sagaId);

                return await ExecuteSaga(
                    @event,
                    model,
                    saga,
                    SagaID.From(saga?.Data?.ID ?? sagaId));
            }
            catch
            {
                if (newSaga != null)
                    await sagaPersistance.Remove(newSaga.Data.ID);

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
                var stateChanged = false;

                internalMessageBus.Subscribe<StateChangedMessage>(this, mesage =>
                {
                    if (mesage.SagaID == id &&
                        mesage.CurrentState == new TState().GetStateName())
                        stateChanged = true;

                    return Task.CompletedTask;
                });

                var saga = await sagaPersistance.Get(id);

                if (saga == null)
                    throw new SagaInstanceNotFoundException(id);

                if (saga.State.CurrentState == new TState().GetStateName())
                    return;

                var stopwatch = Stopwatch.StartNew();
                while (!stateChanged)
                {
                    await Task.Delay(250);
                    if (stopwatch.Elapsed >= waitOptions.Timeout)
                        throw new TimeoutException();
                }
            }
            finally
            {
                internalMessageBus.Unsubscribe<StateChangedMessage>(this);
            }
        }

        private async Task<ISaga> ExecuteSaga(IEvent @event, ISagaModel model, ISaga saga, SagaID id)
        {
            try
            {
                if (saga == null)
                    throw new SagaInstanceNotFoundException(id);

                serviceProvider.GetRequiredService<ObservableRegistrator>().Initialize();

                await internalMessageBus.Publish(new ExecutionStartMessage(saga));

                await sagaPersistance.Set(saga);

                var handler = serviceProvider.GetRequiredService<ExecuteSagaCommandHandler>();

                return await handler.Handle(new ExecuteSagaCommand
                {
                    Async = AsyncExecution.False(),
                    Event = @event,
                    ID = SagaID.From(saga.Data.ID),
                    Model = model
                });
            }
            catch
            {
                await internalMessageBus.Publish(
                    new ExecutionEndMessage(saga));

                throw;
            }
        }

        private async Task<ISaga> CreateNewSagaIfRequired(ISagaModel model, SagaID id, Type eventType)
        {
            ISaga saga = null;

            if (eventType != null)
            {
                var isStartEvent = model.IsStartEvent(eventType);

                if (isStartEvent)
                    saga = await CreateNewSaga(model, id);
            }

            return saga;
        }

        private async Task<ISaga> CreateNewSaga(ISagaModel model, SagaID id)
        {
            if (id == SagaID.Empty())
                id = SagaID.New();

            var data = (ISagaData) Activator.CreateInstance(model.SagaStateType);
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