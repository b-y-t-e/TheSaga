using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.Tests.SagaTests.SyncAndInvalidSaga.EventHandlers;
using TheSaga.Tests.SagaTests.SyncAndInvalidSaga.Events;
using TheSaga.Tests.SagaTests.SyncAndInvalidSaga.Exceptions;
using TheSaga.Tests.SagaTests.SyncAndInvalidSaga.States;

namespace TheSaga.Tests.SagaTests.SyncAndInvalidSaga
{
    public class SyncAndInvalidSagaBuilder : ISagaModelBuilder<SyncAndInvalidSagaData>
    {
        ISagaBuilder<SyncAndInvalidSagaData> builder;

        public SyncAndInvalidSagaBuilder(ISagaBuilder<SyncAndInvalidSagaData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel Build()
        {
            builder.
                Name(nameof(SyncAndInvalidSagaBuilder));

            builder.
                Start<InvalidCreatedEvent, InvalidCreatedEventHandler>().
                Then(
                    "InvalidCreatedEvent1",
                    ctx => Task.CompletedTask,
                    ctx => Task.CompletedTask).
               Then(
                    "InvalidCreatedEvent2",
                    ctx => throw new TestSagaException("error"),
                    ctx => Task.CompletedTask).
                Finish();

            builder.
                Start<ValidCreatedEvent, ValidCreatedEventHandler>().
                TransitionTo<StateCreated>();

            builder.
                During<StateCreated>().
                When<InvalidUpdateEvent>().
                Then(
                    "InvalidUpdateEvent1",
                    ctx => Task.CompletedTask,
                    ctx => Task.CompletedTask).
                Then(
                    "InvalidUpdateEvent2",
                    ctx => Task.CompletedTask,
                    ctx => Task.CompletedTask).
               Then(
                    "InvalidUpdateEvent3",
                    ctx => throw new TestSagaException("error"),
                    ctx => Task.CompletedTask).
                Finish();

            builder.
                During<StateCreated>().
                When<InvalidCompensationEvent>().
                Then(
                    "InvalidCompensationEventStep1",
                    ctx => Task.CompletedTask,
                    ctx => Task.CompletedTask).
                Then(
                    "InvalidCompensationEventStep2",
                    ctx => Task.CompletedTask,
                    ctx => Task.CompletedTask).
                Then(
                    "InvalidCompensationEventStep3",
                    ctx => throw new TestSagaException("error"),
                    ctx => throw new TestCompensationException("compensation error")).
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
