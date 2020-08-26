namespace TheSaga.SagaStates
{
    public static class Extensions
    {
        public static bool IsProcessingCompleted(this ISagaState sagaState)
        {
            return sagaState.CurrentStep == null;
        }
    }
}