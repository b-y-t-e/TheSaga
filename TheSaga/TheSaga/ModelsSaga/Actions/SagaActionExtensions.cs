using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Events;
using TheSaga.Exceptions;
using TheSaga.Models;
using TheSaga.SagaModels.History;
using TheSaga.SagaModels.Steps;
using TheSaga.States;
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
                FindActionsByState(saga.ExecutionState.GetExecutionState());

            if (!eventType.Is<EmptyEvent>())
                return FindStepForEventType(saga, eventType, foundActions);

            return FindStepForCurrentState(saga, foundActions);
        }

        static ISagaStep FindStepForCurrentState(
            ISaga saga, ISagaActions actions)

        {
            ISagaAction action = actions.
                FindActionByStep(saga.ExecutionState.CurrentStep);

            if (action == null)
                throw new SagaStepNotRegisteredException(saga.ExecutionState.GetExecutionState(), saga.ExecutionState.CurrentStep);

            ISagaStep step = action.
                GetStep(saga.ExecutionState.CurrentStep);

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.ExecutionState.GetExecutionState(), saga.ExecutionState.CurrentStep);

            return step;
        }

        static ISagaStep FindStepForEventType(
            ISaga saga, Type eventType, ISagaActions actions)

        {
            ISagaAction action = actions.
                FindActionByEventType(eventType);

            if (action == null)
                throw new SagaInvalidEventForStateException(saga.ExecutionState.GetExecutionState(), eventType);

            ISagaStep step = action.
                ChildSteps.GetFirstStep();

            if (step == null)
                throw new SagaStepNotRegisteredException(saga.ExecutionState.GetExecutionState(), saga.ExecutionState.CurrentStep);

            return step;
        }
        public static ISagaStep GetNextStepToExecute(
            this ISagaAction sagaAction, ISagaStep currentStep, SagaExecutionState sagaState)
        {
            IStepData currentStepData = sagaState.History.
                GetLatestByStepName(currentStep.StepName);

            if (currentStepData?.ExecutionData?.StepType?.Is(typeof(SagaStepIf<,>)) == true)
            {
                if (currentStepData.ExecutionData.ConditionResult == true)
                {
                    return currentStep.ChildSteps.GetFirstStep();
                }
                else
                {
                    return GetNextStep(sagaAction.ChildSteps, currentStep.ParentStep, currentStep);
                }
            }

            if (currentStep.ChildSteps.Any())
                return currentStep.ChildSteps.GetFirstStep();

            ISagaStep nextStep = GetNextStep(sagaAction.ChildSteps, currentStep.ParentStep, currentStep);

            if (nextStep.Is(typeof(SagaStepElse<>))/* &&
                nextStep.ParentStep != currentStep.ParentStep*/)
            {
                ISagaStep prevStepIf = GetPrevStepSameLevel(sagaAction.ChildSteps, currentStep.ParentStep, currentStep);
                if (prevStepIf.Is(typeof(SagaStepIf<,>)))
                {
                    IStepData stepDataIf = sagaState.History.
                        GetLatestByStepName(prevStepIf.StepName);

                    if (stepDataIf?.ExecutionData?.ConditionResult == true)
                        nextStep = GetNextStep(sagaAction.ChildSteps, prevStepIf.ParentStep, prevStepIf);
                }
            }

            return nextStep;
        }

        static ISagaStep GetNextStepSameLevel(
            SagaSteps SagaSteps, ISagaStep parentStep, ISagaStep stepToFind)
        {
            return GetNextStep(SagaSteps, parentStep, stepToFind, true);
        }

        static ISagaStep GetNextStep(
            SagaSteps SagaSteps, ISagaStep parentStep, ISagaStep stepToFind, Boolean onTheSameLevel = false)
        {
            bool stepFound = false;
            if (parentStep == null)
            {
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
                foreach (ISagaStep childStep in parentStep.ChildSteps)
                {
                    if (stepFound)
                        return childStep;

                    if (childStep == stepToFind)
                        stepFound = true;
                }

                if (stepFound && onTheSameLevel)
                    return null;

                return GetNextStep(SagaSteps, stepToFind.ParentStep.ParentStep, stepToFind.ParentStep, onTheSameLevel);
            }
        }

        static ISagaStep GetPrevStepSameLevel(
            SagaSteps SagaSteps, ISagaStep parentStep, ISagaStep stepToFind)
        {
            ISagaStep prevStep = null;
            if (parentStep == null)
            {
                foreach (ISagaStep childStep in SagaSteps)
                {
                    if (childStep == stepToFind)
                    {
                        return prevStep;
                    }
                    prevStep = childStep;
                }
                return null;
            }
            else
            {
                foreach (ISagaStep childStep in parentStep.ChildSteps)
                {
                    if (childStep == stepToFind)
                    {
                        return prevStep;
                    }
                    prevStep = childStep;
                }

                return GetPrevStepSameLevel(SagaSteps, stepToFind.ParentStep.ParentStep, stepToFind.ParentStep);
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