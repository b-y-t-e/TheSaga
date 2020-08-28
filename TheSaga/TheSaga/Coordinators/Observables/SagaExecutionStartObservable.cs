using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheSaga.Exceptions;
using TheSaga.Execution.Actions;
using TheSaga.Execution.AsyncHandlers;
using TheSaga.InternalMessages;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.SagaStates;
using TheSaga.Utils;

namespace TheSaga.Coordinators.AsyncHandlers
{
    internal class SagaExecutionStartObservable : IObservable

    {
        IServiceProvider serviceProvider;

        public SagaExecutionStartObservable(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        async Task OnSagaProcessingStart(SagaExecutionStartMessage msg)
        {
            ISaga saga = msg.Saga;

            if (!saga.IsIdle())
                return;

            saga.State.CurrentError = null;
            saga.State.ExecutionID = ExecutionID.New();
            saga.State.History.Clear();
        }

        public void Subscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IInternalMessageBus>();

            internalMessageBus.
                Subscribe<SagaExecutionStartMessage>(this, OnSagaProcessingStart);
        }

        public void Unsubscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IInternalMessageBus>();

            internalMessageBus.
                Unsubscribe<SagaExecutionStartMessage>(this);
        }
    }
}