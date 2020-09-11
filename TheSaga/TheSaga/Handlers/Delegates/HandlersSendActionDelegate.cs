using System;
using System.Threading.Tasks;
using TheSaga.Events;
using TheSaga.ExecutionContext;
using TheSaga.Handlers.Events;

namespace TheSaga.Handlers.Delegates
{
    public delegate Task HandlersSendActionDelegate<TEvent>(IExecutionContext<HandlersData> context, TEvent @event)
        where TEvent : IHandlersEvent;
}