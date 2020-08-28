using System.Threading.Tasks;

namespace TheSaga.Models.Context
{
    public interface IExecutionContext<TSagaData> : IExecutionContext
        where TSagaData : ISagaData
    {
        TSagaData Data { get; }

        SagaInfo Info { get; }

        SagaState State { get; }

        internal Task Stop();
    }

    public interface IExecutionContext
    {
    }
}