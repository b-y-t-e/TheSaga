using TheSaga.Events;
using TheSaga.Models;
using TheSaga.Models.Interfaces;
using TheSaga.ModelsSaga.Interfaces;
using TheSaga.ValueObjects;

namespace TheSaga.Commands
{
    internal class ExecuteActionCommand
    {
        public AsyncExecution Async;
        public ISagaModel Model;
        public ISaga Saga { get; internal set; }
    }
}
