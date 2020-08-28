﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.MessageBus;
using TheSaga.Messages;
using TheSaga.Models;
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
            IMessageBus internalMessageBus = serviceProvider.GetRequiredService<IMessageBus>();

            internalMessageBus.Subscribe<ExecutionStartMessage>(this, OnSagaProcessingStart);
        }

        public void Unsubscribe()
        {
            IMessageBus internalMessageBus = serviceProvider.GetRequiredService<IMessageBus>();

            internalMessageBus.Unsubscribe<ExecutionStartMessage>(this);
        }

        private async Task OnSagaProcessingStart(ExecutionStartMessage msg)
        {
            ISaga saga = msg.Saga;

            if (!saga.IsIdle())
                return;

            saga.State.CurrentError = null;
            saga.State.ExecutionID = ExecutionID.New();
            saga.State.History.Clear();
        }
    }
}