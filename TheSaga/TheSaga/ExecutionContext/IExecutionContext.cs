using System.Threading.Tasks;
using TheSaga.Models;

namespace TheSaga.ExecutionContext
{
    public interface IExecutionContext<TSagaData> : IExecutionContext
        where TSagaData : ISagaData
    {
        TSagaData Data { get; }

        SagaExecutionInfo Info { get; }

        SagaExecutionState State { get; }

        internal Task Stop();
    }

    public interface IExecutionContext
    {
    }
    public interface IHandlersExecutionContext
    {
        SagaExecutionInfo Info { get; }

        SagaExecutionState State { get; }

        internal Task Stop();
    }

}