using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.Steps;
using TheSaga.Utils;

namespace TheSaga.SagaModels
{
    internal class SagaModel<TSagaData> : ISagaModel<TSagaData>
        where TSagaData : ISagaData
    {
        public SagaModel()
        {
            Name = $"{typeof(TSagaData).Name}Model";
            Actions = new SagaActions<TSagaData>();
            SagaStateType = typeof(TSagaData);
        }

        public SagaActions<TSagaData> Actions { get; }
        public string Name { get; set; }
        public Type SagaStateType { get; }

        public bool ContainsEvent(Type type)
        {
            return
                Actions.StartEvents.Contains(type) ||
                Actions.DuringEvents.Contains(type);
        }

        public ISagaAction FindActionForStateAndEvent(string state, Type eventType)
        {
            SagaAction<TSagaData> action = Actions.FindActionByStateAndEventType(state, eventType);

            if (action == null)
                throw new Exception($"Could not find action for state {state} and event of type {eventType?.Name}");

            return action;
        }

        public ISagaAction FindActionForStep(ISagaStep sagaStep)
        {
            return Actions.FindActionByStep(sagaStep);
        }

        public bool IsStartEvent(Type type)
        {
            return Actions.StartEvents.Contains(type);
        }

        public ISagaStep FindStep(ISaga saga, Type eventType)
        {
            SagaActions<TSagaData> actions = this.
                FindActionsForState(saga.State.GetExecutionState());

            if (!eventType.Is<EmptyEvent>())
                return FindStepForEventType(saga, eventType, actions);
            return FindStepForCurrentState(saga, actions);
        }

        SagaActions<TSagaData> FindActionsForState(string state)
        {
            return Actions.FindActionsByState(state);
        }

        ISagaStep FindStepForCurrentState(ISaga saga, SagaActions<TSagaData> actions)
        {
            ISagaAction action = actions.
                FindActionByStep(saga.State.CurrentStep);

            if (action == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            ISagaStep step = action.
                FindStep(saga.State.CurrentStep);

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            return step;
        }

        ISagaStep FindStepForEventType(ISaga saga, Type eventType, SagaActions<TSagaData> actions)
        {
            ISagaAction action = actions.
                FindActionByEventType(eventType);

            if (action == null)
                throw new SagaInvalidEventForStateException(saga.State.GetExecutionState(), eventType);

            ISagaStep step = action.
                FindFirstStep();

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            return step;
        }
    }
}