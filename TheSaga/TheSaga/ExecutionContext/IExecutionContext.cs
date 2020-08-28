using System.Threading.Tasks;
using TheSaga.Models;

namespace TheSaga.ExecutionContext
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