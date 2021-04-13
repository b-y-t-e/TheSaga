using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Models.History;

namespace TheSaga.ModelsSaga.Steps.Interfaces
{
    public interface ISagaStep
    {
        SagaSteps ChildSteps { get; }
        ISagaStep ParentStep { get; set; }
        bool Async { get; set; }
        string StepName { get; set; }
        Task Compensate(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData);
        Task Execute(IServiceProvider serviceProvider, IExecutionContext context, ISagaEvent @event, IStepData stepData);
    }
}
