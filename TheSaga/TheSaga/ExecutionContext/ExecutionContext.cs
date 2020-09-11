using System.Threading.Tasks;
using TheSaga.Exceptions;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.ExecutionContext
{
    public class ExecutionContext<TSagaData> : IExecutionContext<TSagaData>
        where TSagaData : ISagaData
    {
        public ExecutionContext(TSagaData data, SagaExecutionInfo info, SagaExecutionState state, SagaExecutionValues executionValues)
        {
            Data = data;
            ExecutionInfo = info;
            ExecutionState = state;
            ExecutionValues = executionValues;
        }

        public TSagaData Data { get; set; }

        public SagaExecutionInfo ExecutionInfo { get; set; }

        public SagaExecutionState ExecutionState { get; set; }

        public SagaExecutionValues ExecutionValues { get; set; }

        Task IExecutionContext<TSagaData>.Stop()
        {
            throw new SagaStopException();
        }
    }
}
