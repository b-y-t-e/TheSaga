using TheSaga.Models;

namespace TheSaga.Execution.Commands.Handlers
{
    internal class ExecuteActionResult
    {
        public bool IsSyncProcessingComplete;

        public ISaga Saga;
    }
}