using System.Collections.Generic;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.SagaModels.Actions;
using TheSaga.ValueObjects;

namespace TheSaga.SagaModels.Steps
{
    public interface ISagaStep 
    {
        SagaSteps ChildSteps { get; }
        ISagaStep ParentStep { get; }
        bool Async { get; }
        string StepName { get; }
        Task Compensate(IExecutionContext context, ISagaEvent @event);
        Task Execute(IExecutionContext context, ISagaEvent @event);
    }
}