using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.SagaModels;
using TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga.EventHandlers;
using TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga.Events;
using TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga.Exceptions;
using TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga.States;

namespace TheSaga.Tests.SagaTests.Sagas.AsyncAndInvalidSaga
{
    public class InvalidSagaBuilder : ISagaModelBuilder<InvalidSagaData>
    {
        ISagaBuilder<InvalidSagaData> builder;

        public InvalidSagaBuilder(ISagaBuilder<InvalidSagaData> builder)
        {
            this.builder = builder;
        }

        public ISagaModel<InvalidSagaData> Build()
        {
            builder.
                Name(nameof(InvalidSagaData));

            builder.
                Start<InvalidCreatedEvent, InvalidCreatedEventHandler>().
                ThenAsync(
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
                ThenAsync(
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
                ThenAsync(
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