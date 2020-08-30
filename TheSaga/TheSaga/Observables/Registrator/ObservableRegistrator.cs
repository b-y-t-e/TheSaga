using System;

namespace TheSaga.Observables.Registrator
{
    internal class ObservableRegistrator
    {
        private readonly IServiceProvider serviceProvider;

        private bool wasInitialized;

        public ObservableRegistrator(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            Initialize();
        }

        public void Initialize()
        {
            if (wasInitialized)
                return;

            new LockingObservable(serviceProvider).Subscribe();

            new ExecutionStartObservable(serviceProvider).Subscribe();

            new ExecutionEndObservable(serviceProvider).Subscribe();

            wasInitialized = true;
        }
    }
}