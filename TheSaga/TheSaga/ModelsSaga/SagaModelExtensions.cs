using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.Steps;
using TheSaga.States;
using TheSaga.Utils;

namespace TheSaga.SagaModels
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