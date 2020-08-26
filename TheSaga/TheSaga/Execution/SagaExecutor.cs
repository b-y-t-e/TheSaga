using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Execution.Actions;
using TheSaga.Execution.AsyncHandlers;
using TheSaga.Messages;
using TheSaga.Messages.MessageBus;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;
using TheSaga.States;
using TheSaga.Utils;

namespace TheSaga.Execution
{
    internal class SagaExecutor<TSagaState> : ISagaExecutor
        where TSagaState : ISagaState
    {
        private ISagaModel<TSagaState> model;
        private ISagaPersistance sagaPersistance;
        private IInternalMessageBus internalMessageBus;

        public SagaExecutor(
            ISagaModel<TSagaState> model,
            ISagaPersistance sagaPersistance,
            IInternalMessageBus internalMessageBus)
        {
            this.model = model;
            this.sagaPersistance = sagaPersistance;
            this.internalMessageBus = internalMessageBus;

            new SagaAsyncHandler<TSagaState>(this, sagaPersistance, internalMessageBus).
                Subscribe();
        }

        public async Task<ISagaState> Handle(Guid correlationID, IEvent @event, bool async)
        {
            SagaActionExecutor<TSagaState> actionExecutor =
                new SagaActionExecutor<TSagaState>(correlationID, async, @event, model, internalMessageBus, sagaPersistance);

            ActionExecutionResult stepExecutionResult = await actionExecutor.
                ExecuteAction();

            if (stepExecutionResult.IsSyncProcessingComplete)
                return stepExecutionResult.State;

            return await Handle(correlationID, null, @async);
        }
    }
}