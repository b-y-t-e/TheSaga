using System;
using System.Threading.Tasks;
using TheSaga.ExecutionContext;
using TheSaga.Handlers.Events;

namespace TheSaga.Handlers.Activities
{
    public interface IHandlersActivity
    {
        Task Compensate(IExecutionContext<HandlersData> context);

        Task Execute(IExecutionContext<HandlersData> context);
    }
}