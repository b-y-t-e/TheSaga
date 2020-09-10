using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.ExecutionContext;
using TheSaga.Tests.SagaTests.Sagas.ResumeSaga;

namespace TheSaga.Tests.SagaTests.Sagas.ChildStepsSaga.Activities
{
    internal class InnerActivity1 : ISagaActivity<ChildStepsSagaData>
    {
        public Task Compensate(IExecutionContext<ChildStepsSagaData> context)
        {
            return Task.CompletedTask;
        }

        public Task Execute(IExecutionContext<ChildStepsSagaData> context)
        {
            return Task.CompletedTask;
        }
    }
}