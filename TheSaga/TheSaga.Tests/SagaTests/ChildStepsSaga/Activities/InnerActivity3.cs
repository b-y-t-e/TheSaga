using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.ChildStepsSaga.Activities
{
    internal class InnerActivity3 : ISagaActivity<ChildStepsSagaData>
    {
        public async Task Compensate(IExecutionContext<ChildStepsSagaData> context)
        {
        }

        public async Task Execute(IExecutionContext<ChildStepsSagaData> context)
        {
        }
    }
}
