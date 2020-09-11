using System;
using System.Linq;
using TheSaga.ModelsSaga.Actions;
using TheSaga.ModelsSaga.Actions.Interfaces;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.ModelsSaga.Steps.Interfaces;

namespace TheSaga.ModelsSaga
{
    internal static class SagaModelExtensions
    {
        public static SagaActions FindActionsByState(
            this ISagaModel model, string state)
        {
            return new SagaActions(
                model.Actions.Where(s => s.State == state));
        }
        public static ISagaAction FindActionForStateAndEvent(
            this ISagaModel model, string state, Type eventType)
            
        {
            ISagaAction action = model.FindActionByStateAndEventType(state, eventType);

            if (action == null)
                throw new Exception($"Could not find action for state {state} and event of type {eventType?.Name}");

            return action;
        }

        public static ISagaAction FindActionForStep(
            this ISagaModel model, ISagaStep sagaStep)
            
        {
            return model.Actions.
                FindActionByStep(sagaStep?.StepName);
        }

        public static ISagaAction FindActionByStateAndEventType(
            this ISagaModel model, string stateName, Type eventType)

        {
            return model.Actions.FindActionByStateAndEventType(stateName, eventType);
        }
    }
}
