namespace TheSaga.Models
{
    public static class Extensions
    {
        public static bool IsIdle(this ISaga sagaState)
        {
            return sagaState.ExecutionState.CurrentStep == null;
        }

        public static bool HasError(this ISaga sagaState)
        {
            return sagaState.ExecutionState.CurrentError != null;
        }
    }
}