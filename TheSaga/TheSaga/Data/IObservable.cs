namespace TheSaga.Execution.AsyncHandlers
{
    internal interface IObservable
    {
        void Subscribe();

        void Unsubscribe();
    }
}