namespace TheSaga.SagaStates
{
    public static class Extensions
    {
        public static bool IsIdle(this ISagaState sagaState)
        {
            return sagaState.State.CurrentStep == null;
        }

        public static bool HasError(this ISagaState sagaState)
        {
            return sagaState.State.CurrentError != null;
        }
    }
}