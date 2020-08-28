namespace TheSaga.Observables
{
    internal interface IObservable
    {
        void Subscribe();

        void Unsubscribe();
    }
}