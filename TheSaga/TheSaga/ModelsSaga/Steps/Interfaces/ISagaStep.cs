using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.History;
using TheSaga.ValueObjects;

namespace TheSaga.SagaModels.Steps
{
    public interface ISagaStep 
    {
        SagaSteps ChildSteps { get; }
        ISagaStep ParentStep { get; }
        bool Async { get; }
        string StepName { get; }
        Task Compensate(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData);
        Task Execute(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData);
    }
}