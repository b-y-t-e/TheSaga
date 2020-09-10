using System.Threading.Tasks;
using TheSaga.Exceptions;
using TheSaga.Models;

namespace TheSaga.ExecutionContext
{
    public class ExecutionContext<TSagaData> : IExecutionContext<TSagaData>
        where TSagaData : ISagaData
    {
        public ExecutionContext(TSagaData data, SagaExecutionInfo info, SagaExecutionState state)
        {
            Data = data;
            Info = info;
            State = state;
        }

        public TSagaData Data { get; set; }

        public SagaExecutionInfo Info { get; set; }

        public SagaExecutionState State { get; set; }

        Task IExecutionContext<TSagaData>.Stop()
        {
            throw new SagaStopException();
        }
    }
}