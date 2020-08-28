using TheSaga.Models;

namespace TheSaga.Execution.Context
{
    public interface IExecutionContext<TSagaData> : IExecutionContext
        where TSagaData : ISagaData
    {
        TSagaData Data { get; }

        SagaInfo Info { get; }

        SagaState State { get; }
    }

    public interface IExecutionContext
    {
    }
}