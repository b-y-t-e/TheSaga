using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Messages;
using TheSaga.Messages.MessageBus;
using TheSaga.Models;
using TheSaga.ValueObjects;

namespace TheSaga.Coordinators.Observables
{
    internal class ExecutionStartObservable : IObservable

    {
        IServiceProvider serviceProvider;

        public ExecutionStartObservable(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        async Task OnSagaProcessingStart(ExecutionStartMessage msg)
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
                Subscribe<ExecutionStartMessage>(this, OnSagaProcessingStart);
        }

        public void Unsubscribe()
        {
            var internalMessageBus = serviceProvider.
                GetRequiredService<IInternalMessageBus>();

            internalMessageBus.
                Unsubscribe<ExecutionStartMessage>(this);
        }
    }
}