using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Coordinators;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models.Interfaces;
using TheSaga.Models.History;
using TheSaga.ModelsSaga.Steps.Delegates;
using TheSaga.ModelsSaga.Steps.Interfaces;
using TheSaga.Models;

namespace TheSaga.ModelsSaga.Steps
{
    internal class SagaStepForBreak<TSagaData> : ISagaStep
        where TSagaData : ISagaData
    {
        public SagaSteps ChildSteps { get; }
        public ISagaStep ParentStep { get; }

        public SagaStepForBreak(
            string StepName, bool async, ISagaStep parentStep)
        {
            this.StepName = StepName;
            Async = async;
            ChildSteps = new SagaSteps();
            ParentStep = parentStep;
        }

        public bool Async { get; }
        public string StepName { get; }

        public async Task Compensate(
            IServiceProvider serviceProvider,
            IExecutionContext context,
            ISagaEvent @event,
            IStepData stepData)
        {
            

        }

        public async Task Execute(
            IServiceProvider serviceProvider,
            IExecutionContext context,
            ISagaEvent @event,
            IStepData stepData)
        {
            (context.ExecutionState as SagaExecutionState).IsBreaked = true;
        }
    }
}
