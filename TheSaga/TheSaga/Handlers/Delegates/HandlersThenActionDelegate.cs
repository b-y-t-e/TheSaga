using System.Threading.Tasks;
using TheSaga.ExecutionContext;
using TheSaga.Handlers.Events;

namespace TheSaga.Handlers.Delegates
{
    public delegate Task HandlersThenActionDelegate(IExecutionContext<HandlersData> context);
}