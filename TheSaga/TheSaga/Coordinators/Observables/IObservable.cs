namespace TheSaga.Coordinators.Observables
{
    internal interface IObservable
    {
        void Subscribe();

        void Unsubscribe();
    }
}