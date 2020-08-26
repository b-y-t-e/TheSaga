using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Execution.Actions;
using TheSaga.Execution.AsyncHandlers;
using TheSaga.InternalMessages.MessageBus;
using TheSaga.Models;
using TheSaga.Providers;
using TheSaga.SagaStates;

namespace TheSaga.Execution
{
    internal class SagaExecutor<TSagaState> : ISagaExecutor<TSagaState>
        where TSagaState : ISagaState
    {
        private ISagaModel<TSagaState> model;
        private IServiceProvider serviceProvider;

        public SagaExecutor(
            ISagaModel model,
            IServiceProvider serviceProvider)
        {
            this.model = (ISagaModel<TSagaState>)model;
            this.serviceProvider = serviceProvider;
        }

        public async Task<ISagaState> Handle(Guid correlationID, IEvent @event, IsExecutionAsync async)
        {
            try
            {
                if (@event == null)
                    @event = new EmptyEvent();

                SagaActionExecutor<TSagaState> actionExecutor = ActivatorUtilities.
                   CreateInstance<SagaActionExecutor<TSagaState>>(serviceProvider, correlationID, async, @event, model);

                ActionExecutionResult stepExecutionResult = await actionExecutor.
                    ExecuteAction();

                if (stepExecutionResult.IsSyncProcessingComplete)
                    return stepExecutionResult.State;

                return await Handle(correlationID, null, @async);
            }
            catch
            {
                throw;
            }
        }
    }
}