using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;

namespace TheSaga.Tests.SagaTests.ResumeSaga.Events
{
    public class CreateNewSagaHandler1 : ISagaEventHandler<ResumeSagaData, CreateNewSaga>
    {
        public async Task Compensate(IExecutionContext<ResumeSagaData> context, CreateNewSaga @event)
        {

        }

        public async Task Execute(IExecutionContext<ResumeSagaData> context, CreateNewSaga @event)
        {   

        }
    }

    public class CreateNewSagaHandler2 : ISagaEventHandler<ResumeSagaData, CreateNewSaga>
    {
        public async Task Compensate(IExecutionContext<ResumeSagaData> context, CreateNewSaga @event)
        {

        }

        public async Task Execute(IExecutionContext<ResumeSagaData> context, CreateNewSaga @event)
        {

        }
    }

    public class CreateNewSagaHandler3 : ISagaEventHandler<ResumeSagaData, CreateNewSaga>
    {
        public async Task Compensate(IExecutionContext<ResumeSagaData> context, CreateNewSaga @event)
        {
            if (ResumeSagaSettings.StopSagaExecution) await context.Stop();
        }

        public async Task Execute(IExecutionContext<ResumeSagaData> context, CreateNewSaga @event)
        {
            if (ResumeSagaSettings.StopSagaExecution) throw new System.Exception("!!");
            //if (ResumeSagaSettings.ThrowError) 
        }
    }
}
