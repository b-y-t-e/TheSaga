using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.ExecutionContext;
using TheSaga.Tests.SagaTests.Sagas.SyncAndValid;

namespace TheSaga.Tests.SagaTests.Sagas.ChildStepsSaga.Activities
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