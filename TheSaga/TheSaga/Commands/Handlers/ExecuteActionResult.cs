using TheSaga.Models;

namespace TheSaga.Commands.Handlers
{
    internal class ExecuteActionResult
    {
        public bool IsSyncProcessingComplete;

        public ISaga Saga;
    }
}