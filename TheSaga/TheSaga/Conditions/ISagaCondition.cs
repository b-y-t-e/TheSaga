using System;
using System.Threading.Tasks;
using TheSaga.ExecutionContext;
using TheSaga.Models;

namespace TheSaga.Conditions
{
    public interface ISagaCondition<TSagaData> : ISagaCondition
        where TSagaData : ISagaData
    {
        Task Compensate(IExecutionContext<TSagaData> context);

        Task<bool> Execute(IExecutionContext<TSagaData> context);
    }

    public interface ISagaCondition
    {
    }
}