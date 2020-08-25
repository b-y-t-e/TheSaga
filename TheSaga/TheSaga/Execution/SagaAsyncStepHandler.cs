using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Messages;
using TheSaga.Messages.MessageBus;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;
using TheSaga.States;

namespace TheSaga.Execution
{
    internal class SagaAsyncStepHandler<TSagaState> 
        where TSagaState : ISagaState
    {
        private ISagaExecutor sagaExecutor;
        private ISagaPersistance sagaPersistance;
        private IInternalMessageBus internalMessageBus;

        public SagaAsyncStepHandler(ISagaExecutor sagaExecutor, ISagaPersistance sagaPersistance, IInternalMessageBus internalMessageBus)
        {
            this.sagaExecutor = sagaExecutor;
            this.sagaPersistance = sagaPersistance;
            this.internalMessageBus = internalMessageBus;
        }

        public void Subscribe()
        {
            this.internalMessageBus.
                Subscribe<SagaStepCompletedAsyncMessage>(this, HandleAsyncStepCompletedMessage);
        }

        private Task HandleAsyncStepCompletedMessage(SagaStepCompletedAsyncMessage message)
        {
            if (message.SagaStateType != typeof(TSagaState))
                return Task.CompletedTask;

            Task.Run(async () =>
            {
                try
                {
                    await sagaExecutor.Handle(message.CorrelationID, null, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    try
                    {
                        var sagaState = await sagaPersistance.Get(message.CorrelationID);
                        sagaState.CurrentError = ex.Message;
                        await sagaPersistance.Set(sagaState);
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine(ex2);
                    }
                }
            });

            return Task.CompletedTask;
        }

    }
}