using System;
using System.Threading;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Coordinators;
using TheSaga.Executors;
using TheSaga.Interfaces;
using TheSaga.Models;
using TheSaga.Registrator;
using TheSaga.Persistance;
using TheSaga.States;
using Xunit;
using Xunit.Sdk;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TheSaga.Exceptions;
using TheSaga.Tests.Sagas.AsyncSaga;
using TheSaga.Tests.Sagas.AsyncSaga.Events;
using TheSaga.Tests.Sagas.AsyncSaga.States;
using TheSaga.Tests.Sagas.AsyncSaga.EventHandlers;

namespace TheSaga.Tests
{
    public class AsyncSagaStateTests
    {
        IServiceProvider serviceProvider;

        [Fact]
        public async Task WHEN_sagaContainsAsyncMethods_THEN_stepsShouldBeExecutedAsync()
        {
            // given
            ISagaPersistance sagaPersistance = serviceProvider.
                GetRequiredService<ISagaPersistance>();

            ISagaCoordinator sagaCoordinator = serviceProvider.
                GetRequiredService<ISagaCoordinator>();

            IEvent startEvent = new CreatedEvent();

            // when
            ISagaState sagaState = await sagaCoordinator.
                Send(startEvent);

            // then
            AsyncState persistedState = (AsyncState)await sagaPersistance.
                Get(sagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.CurrentStep.ShouldStartWith("~");
            persistedState.CurrentState.ShouldBe("~");
            persistedState.CorrelationID.ShouldBe(sagaState.CorrelationID);
            persistedState.Logs.ShouldContain(nameof(CreatedEventHandler));
            persistedState.Logs.Count.ShouldBe(1);
        }

        [Fact]
        public async Task WHEN_abc_THEN_def()
        {
            // given
            ISagaPersistance sagaPersistance = serviceProvider.
                GetRequiredService<ISagaPersistance>();

            ISagaCoordinator sagaCoordinator = serviceProvider.
                GetRequiredService<ISagaCoordinator>();

            IEvent startEvent = new CreatedEvent();

            // when
            ISagaState sagaState = await sagaCoordinator.
                Send(startEvent);

            // then
            AsyncState persistedState = (AsyncState)await sagaPersistance.
                Get(sagaState.CorrelationID);

            persistedState.ShouldNotBeNull();
            persistedState.CurrentStep.ShouldStartWith("~");
            persistedState.CurrentState.ShouldBe("~");
            persistedState.CorrelationID.ShouldBe(sagaState.CorrelationID);
            persistedState.Logs.ShouldContain(nameof(CreatedEventHandler));
            persistedState.Logs.Count.ShouldBe(1);
        }

        public AsyncSagaStateTests()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<ISagaPersistance, InMemorySagaPersistance>();
            services.AddScoped<ISagaRegistrator, SagaRegistrator>();
            services.AddScoped<ISagaCoordinator, SagaCoordinator>();
            serviceProvider = services.BuildServiceProvider();

            ISagaRegistrator sagaRegistrator = serviceProvider.
                GetRequiredService<ISagaRegistrator>();

            sagaRegistrator.Register(
                new AsyncSagaDefinition().GetModel(serviceProvider));

        }
    }

}
