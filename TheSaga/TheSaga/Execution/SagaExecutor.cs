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
    internal class SagaExecutor<TSagaData> : ISagaExecutor<TSagaData>
        where TSagaData : ISagaData
    {
        private ISagaModel<TSagaData> model;
        private IServiceProvider serviceProvider;

        public SagaExecutor(
            ISagaModel model,
            IServiceProvider serviceProvider)
        {
            this.model = (ISagaModel<TSagaData>)model;
            this.serviceProvider = serviceProvider;
        }

        public async Task<ISaga> Handle(Guid correlationID, IEvent @event, IsExecutionAsync async)
        {
            try
            {
                if (@event == null)
                    @event = new EmptyEvent();

                SagaActionExecutor<TSagaData> actionExecutor = ActivatorUtilities.
                   CreateInstance<SagaActionExecutor<TSagaData>>(serviceProvider, correlationID, async, @event, model);

                ActionExecutionResult stepExecutionResult = await actionExecutor.
                    ExecuteAction();

                if (stepExecutionResult.IsSyncProcessingComplete)
                    return stepExecutionResult.Saga;

                return await Handle(correlationID, null, @async);
            }
            catch
            {
                throw;
            }
        }
    }
}