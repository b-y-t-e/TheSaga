using System;
using System.Data;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.Models;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.EventHandlers;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.Events;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.Exceptions;
using TheSaga.Tests.Sagas.SyncAndInvalidSaga.States;

namespace TheSaga.Tests.Sagas.SyncAndInvalidSaga
{
    public class InvalidSagaDefinition : ISagaModelDefintion<InvalidSagaState>
    {
        public ISagaModel<InvalidSagaState> GetModel(IServiceProvider serviceProvider)
        {
            ISagaBuilder<InvalidSagaState> builder = new SagaBuilder<InvalidSagaState>(serviceProvider);

            builder.
                Start<InvalidCreatedEvent, InvalidCreatedEventHandler>().
                Then(
                    ctx =>
                    {
                        ctx.State.Logs.Add("execution1");
                        return Task.CompletedTask;
                    },
                    ctx =>
                    {
                        ctx.State.Logs.Add("compensation1");
                        return Task.CompletedTask;
                    }).
               Then(
                    ctx =>
                    {
                        ctx.State.Logs.Add("execution2");
                        throw new TestSagaException("error");
                    },
                    ctx =>
                    {
                        ctx.State.Logs.Add("compensation2");
                        return Task.CompletedTask;
                    }).
                Finish();

            builder.
                Start<ValidCreatedEvent, ValidCreatedEventHandler>().
                TransitionTo<StateCreated>();

            builder.
                During<StateCreated>().
                When<InvalidUpdateEvent>().
                Then(
                    ctx =>
                    {
                        ctx.State.Logs.Add("execution1");
                        return Task.CompletedTask;
                    },
                    ctx =>
                    {
                        ctx.State.Logs.Add("compensation1");
                        return Task.CompletedTask;
                    }).
                Then(
                    ctx =>
                    {
                        ctx.State.Logs.Add("execution2");
                        return Task.CompletedTask;
                    },
                    ctx =>
                    {
                        ctx.State.Logs.Add("compensation2");
                        return Task.CompletedTask;
                    }).
               Then(
                    ctx =>
                    {
                        ctx.State.Logs.Add("execution3");
                        throw new TestSagaException("error");
                    },
                    ctx =>
                    {
                        ctx.State.Logs.Add("compensation3");
                        return Task.CompletedTask;
                    }).
                Finish();

            builder.
                During<StateCreated>().
                When<InvalidCompensationEvent>().
                Then(
                    ctx =>
                    {
                        ctx.State.Logs.Add("execution1");
                        return Task.CompletedTask;
                    },
                    ctx =>
                    {
                        ctx.State.Logs.Add("compensation1");
                        return Task.CompletedTask;
                    }).
                Then(
                    ctx =>
                    {
                        ctx.State.Logs.Add("execution2");
                        return Task.CompletedTask;
                    },
                    ctx =>
                    {
                        ctx.State.Logs.Add("compensation2");
                        return Task.CompletedTask;
                    }).
               Then(
                    ctx =>
                    {
                        ctx.State.Logs.Add("execution3");
                        throw new TestSagaException("error");
                    },
                    ctx =>
                    {
                        ctx.State.Logs.Add("compensation3");
                        throw new TestCompensationException("compensation error");
                    }).
                Finish();

            builder.
                During<StateCreated>().
                When<ValidUpdateEvent>().
                TransitionTo<StateUpdated>();

            return builder.
                Build();
        }
    }
}