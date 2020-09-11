using System;
using System.Threading.Tasks;
using TheSaga.Models;

namespace TheSaga.Handlers.ExecutionContext
{
    public interface IHandlersExecutionContext
    {
        SagaExecutionInfo Info { get; }

        SagaExecutionState State { get; }

        internal Task Stop();
    }
}