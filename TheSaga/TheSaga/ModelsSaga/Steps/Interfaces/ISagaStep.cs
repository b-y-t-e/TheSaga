using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.ModelsSaga.History;

namespace TheSaga.ModelsSaga.Steps.Interfaces
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
