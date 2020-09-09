using System;
using System.Threading.Tasks;
using TheSaga.ExecutionContext;
using TheSaga.Models;

namespace TheSaga.Events
{
    public interface ISagaEventHandler<TSagaData, TEvent> : ISagaEventHandler
        where TSagaData : ISagaData
        where TEvent : ISagaEvent
    {
        Task Compensate(IExecutionContext<TSagaData> context, TEvent @event);

        Task Execute(IExecutionContext<TSagaData> context, TEvent @event);
    }

    public interface ISagaEventHandler
    {
    }

    public interface IHandlersCompensateEventHandler<TEvent> : ISagaEventHandler
        where TEvent : IHandlersEvent
    {
        Task Compensate(IHandlersExecutionContext context, TEvent @event);

        Task Execute(IHandlersExecutionContext context, TEvent @event);
    }

    public class HandlersData : ISagaData
    {
        public Guid ID { get; set; }
    }
}