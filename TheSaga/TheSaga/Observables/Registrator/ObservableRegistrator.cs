using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TheSaga.Exceptions;
using TheSaga.Locking;
using TheSaga.Messages;
using TheSaga.Messages.MessageBus;

namespace TheSaga.Observables.Registrator
{
    internal class ObservableRegistrator 
    {
        private IServiceProvider serviceProvider;

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

            new LockingObservable(serviceProvider).
                Subscribe();

            new ExecutionStartObservable(serviceProvider).
                Subscribe();

            new ExecutionEndObservable(serviceProvider).
                Subscribe();

            new AsyncStepCompletedObservable(serviceProvider).
                Subscribe();

            wasInitialized = true;
        }
    }
}