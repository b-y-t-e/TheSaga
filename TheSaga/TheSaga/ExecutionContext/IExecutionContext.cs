using System.Threading.Tasks;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.ExecutionContext
{
    public interface IExecutionContext<TSagaData> : IExecutionContext
        where TSagaData : ISagaData
    {
        TSagaData Data { get; }

        SagaExecutionInfo ExecutionInfo { get; }

        SagaExecutionState ExecutionState { get; }

        SagaExecutionValues ExecutionValues { get; }

        internal Task Stop();
    }

    public interface IExecutionContext
    {
    }
}
