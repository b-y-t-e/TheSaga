﻿using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models.History;
using TheSaga.ModelsSaga.Steps.Interfaces;

namespace TheSaga.ModelsSaga.Steps
{
    internal class SagaEmptyStep : ISagaStep
    {
        public SagaSteps ChildSteps { get; private set; }
        public ISagaStep ParentStep { get; set; }
        public bool Async { get; set; }
        public string StepName { get; set; }

        public SagaEmptyStep(
            string StepName, ISagaStep parentStep)
        {
            this.StepName = StepName;
            Async = false;
            ChildSteps = new SagaSteps();
            ParentStep = parentStep;
        }


        public Task Compensate(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData)
        {
            return Task.CompletedTask;
        }
    }
}
