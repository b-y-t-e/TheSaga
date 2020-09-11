namespace TheSaga.Observables.Interfaces
{
    internal interface IObservable
    {
        void Subscribe();

        void Unsubscribe();
    }
}
