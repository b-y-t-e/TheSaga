using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.ExecutionContext;
using TheSaga.Conditions;
using TheSaga.Tests.SagaTests.Sagas.ResumeSaga;

namespace TheSaga.Tests.SagaTests.Sagas.ChildStepsSaga.Conditions
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