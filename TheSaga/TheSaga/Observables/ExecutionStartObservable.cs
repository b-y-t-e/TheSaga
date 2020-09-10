using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.MessageBus;
using TheSaga.Messages;
using TheSaga.Models;
using TheSaga.SagaModels;
using TheSaga.ValueObjects;

namespace TheSaga.Observables
{
    internal class ExecutionStartObservable : IObservable

    {
        private readonly IServiceProvider serviceProvider;

        public ExecutionStartObservable(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void Subscribe()
        {
            IMessageBus messageBus = serviceProvider.GetRequiredService<IMessageBus>();

            messageBus.Subscribe<ExecutionStartMessage>(this, OnSagaProcessingStart);
        }

        public void Unsubscribe()
        {
            IMessageBus messageBus = serviceProvider.GetRequiredService<IMessageBus>();

            messageBus.Unsubscribe<ExecutionStartMessage>(this);
        }

        private async Task OnSagaProcessingStart(ExecutionStartMessage msg)
        {
            ISaga saga = msg.Saga;
            SagaModels.ISagaModel model = msg.Model;

            if (!saga.IsIdle())
                return;

            saga.ExecutionState.CurrentError = null;
            saga.ExecutionState.ExecutionID = ExecutionID.New();

            if (model.HistoryPolicy == ESagaHistoryPolicy.StoreOnlyCurrentStep)
                saga.ExecutionState.History.Clear();
        }
    }
}