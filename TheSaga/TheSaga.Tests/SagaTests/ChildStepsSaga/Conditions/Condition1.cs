using System.Threading.Tasks;
using TheSaga.Conditions;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.ChildStepsSaga.Conditions
{
    internal class Condition1 : ISagaCondition<ChildStepsSagaData>
    {
        public Task Compensate(IExecutionContext<ChildStepsSagaData> context)
        {
            return Task.CompletedTask;
        }

        public Task<bool> Execute(IExecutionContext<ChildStepsSagaData> context)
        {
            return Task.FromResult(true);
        }
    }
}
