using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Execution.Actions;
using TheSaga.Models;
using TheSaga.Persistance;
using TheSaga.SagaStates;
using TheSaga.SagaStates.Actions;
using TheSaga.SagaStates.Steps;
using TheSaga.Utils;

namespace TheSaga.Execution.Commands
{
    internal class ExecuteActionCommandHandler<TSagaData>
        where TSagaData : ISagaData
    {
        private ISagaPersistance sagaPersistance;
        private IServiceProvider serviceProvider;

        public ExecuteActionCommandHandler(
            ISagaPersistance sagaPersistance,
            IServiceProvider serviceProvider)
        {
            this.sagaPersistance = sagaPersistance;
            this.serviceProvider = serviceProvider;
        }

        public async Task<ExecuteActionResult> Handle(ExecuteActionCommand<TSagaData> command)
        {
            if (command.Event == null)
                command.Event = new EmptyEvent();

            ISaga saga = await sagaPersistance.
                Get(command.ID);

            if (saga == null)
                throw new SagaInstanceNotFoundException(command.Model.SagaStateType, command.ID);

            IList<ISagaAction> actions = command.Model.FindActionsForState(saga.State.GetExecutionState());
            ISagaStep step = FindStep(saga, command.Event.GetType(), actions);
            ISagaAction action = command.Model.FindActionForStep(step);

            IsExecutionAsync async = IsExecutionAsync.From(step.Async);
            if (step.Async)
                async = IsExecutionAsync.True();

            ExecuteStepCommandHandler<TSagaData> stepExecutor = ActivatorUtilities.
               CreateInstance<ExecuteStepCommandHandler<TSagaData>>(serviceProvider);

            await stepExecutor.
                Handle(new ExecuteStepCommand<TSagaData>()
                {
                    async = async,
                    @event = command.Event,
                    saga = saga,
                    sagaStep = step,
                    sagaAction = action
                });

            return new ExecuteActionResult()
            {
                Saga = saga,
                IsSyncProcessingComplete = async || saga.IsIdle()
            };
        }

        private ISagaStep FindStep(ISaga saga, Type eventType, IList<ISagaAction> actions)
        {
            if (!eventType.Is<EmptyEvent>())
            {
                return FindStepForEventType(saga, eventType, actions);
            }
            else
            {
                return FindStepForCurrentState(saga, actions);
            }
        }

        private ISagaStep FindStepForCurrentState(ISaga saga, IList<ISagaAction> actions)
        {
            if (saga.IsIdle())
                throw new Exception("");

            ISagaAction action = actions.
                FirstOrDefault(a => a.FindStep(saga.State.CurrentStep) != null);

            if (action == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            ISagaStep step = action.
                FindStep(saga.State.CurrentStep);

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            return step;
        }

        private ISagaStep FindStepForEventType(ISaga saga, Type eventType, IList<ISagaAction> actions)
        {
            ISagaAction action = actions.
                FirstOrDefault(a => a.Event == eventType);

            if (action == null)
                throw new SagaInvalidEventForStateException(saga.State.GetExecutionState(), eventType);

            if (!saga.IsIdle())
                throw new SagaIsBusyHandlingStepException(saga.Data.ID, saga.State.GetExecutionState(), saga.State.CurrentStep);

            ISagaStep step = action.Steps.FirstOrDefault();

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            return step;
        }
    }
    internal class ExecuteActionResult
    {
        public bool IsSyncProcessingComplete;

        public ISaga Saga;
    }
}