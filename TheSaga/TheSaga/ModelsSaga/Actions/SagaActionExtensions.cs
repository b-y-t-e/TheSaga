using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Models;
using TheSaga.SagaModels.Steps;
using TheSaga.Utils;

namespace TheSaga.SagaModels.Actions
{
    internal static class SagaActionExtensions
    {
        public static ISagaAction FindActionByStep(
            this ISagaActions actions, string step)
        {
            return actions.
                FirstOrDefault(action => action.GetStep(step) != null);
        }
        public static ISagaAction FindActionByEventType(
            this ISagaActions actions, Type eventType)
        {
            return actions.
                FirstOrDefault(action => action.Event == eventType);
        }
        public static ISagaAction FindActionByStateAndEventType(
            this ISagaActions actions, string state, Type eventType)
        {
            return actions.
                FirstOrDefault(action => action.State == state && action.Event == eventType);
        }
        public static ISagaActions FindActionsByState(
            this ISagaActions actions, string state)
        {
            return new SagaActions(
                actions.Where(s => s.State == state));
        }
        public static ISagaStep FindStep(
            this ISagaActions actions, ISaga saga, Type eventType)

        {
            ISagaActions foundActions = actions.
                FindActionsByState(saga.State.GetExecutionState());

            if (!eventType.Is<EmptyEvent>())
                return FindStepForEventType(saga, eventType, foundActions);

            return FindStepForCurrentState(saga, foundActions);
        }

        static ISagaStep FindStepForCurrentState(
            ISaga saga, ISagaActions actions)

        {
            ISagaAction action = actions.
                FindActionByStep(saga.State.CurrentStep);

            if (action == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            ISagaStep step = action.
                GetStep(saga.State.CurrentStep);

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            return step;
        }

        static ISagaStep FindStepForEventType(
            ISaga saga, Type eventType, ISagaActions actions)

        {
            ISagaAction action = actions.
                FindActionByEventType(eventType);

            if (action == null)
                throw new SagaInvalidEventForStateException(saga.State.GetExecutionState(), eventType);

            ISagaStep step = action.
                ChildSteps.GetFirstStep();

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.State.GetExecutionState(), saga.State.CurrentStep);

            return step;
        }
        public static ISagaStep GetNextStep(
            this ISagaAction sagaAction, ISagaStep stepToFind)
        {
            if (stepToFind.ChildSteps.Any())
                return stepToFind.ChildSteps.GetFirstStep();

            return GetNextStep(sagaAction.ChildSteps, stepToFind.ParentStep, stepToFind);
        }

        static ISagaStep GetNextStep(
            SagaSteps SagaSteps, ISagaStep parentStep, ISagaStep stepToFind)
        {
            if (parentStep == null)
            {
                bool stepFound = false;
                foreach (ISagaStep childStep in SagaSteps)
                {
                    if (stepFound)
                        return childStep;

                    if (childStep == stepToFind)
                        stepFound = true;
                }
                return null;
            }
            else
            {
                bool stepFound = false;
                foreach (ISagaStep childStep in parentStep.ChildSteps)
                {
                    if (stepFound)
                        return childStep;

                    if (childStep == stepToFind)
                        stepFound = true;
                }
                return GetNextStep(SagaSteps, stepToFind.ParentStep.ParentStep, stepToFind.ParentStep);
            }
        }

        public static ISagaStep GetStep(this ISagaAction sagaAction, string stepName)
        {
            ISagaStep step = sagaAction.ChildSteps.FirstOrDefault(s => s.StepName == stepName);
            if (step != null)
                return step;

            foreach (var childStep in sagaAction.ChildSteps)
            {
                step = FindChildStep(childStep, stepName);
                if (step != null)
                    return step;
            }
            return null;
        }

        static ISagaStep FindChildStep(ISagaStep parent, string stepName)
        {
            ISagaStep step = parent.ChildSteps.
                FirstOrDefault(s => s.StepName == stepName);

            if (step != null)
                return step;

            foreach (var childStep in parent.ChildSteps)
            {
                step = FindChildStep(childStep, stepName);
                if (step != null)
                    return step;
            }

            return null;
        }
    }
}