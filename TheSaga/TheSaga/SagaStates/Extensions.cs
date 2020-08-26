namespace TheSaga.SagaStates
{
    public static class Extensions
    {
        public static bool IsProcessingCompleted(this ISagaData sagaData)
        {
            return sagaData.SagaState.CurrentStep == null;
        }
    }
}