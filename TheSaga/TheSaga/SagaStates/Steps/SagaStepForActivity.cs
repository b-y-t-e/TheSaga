﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Events;
using TheSaga.Execution.Context;

namespace TheSaga.SagaStates.Steps
{
    public class SagaStepForActivity<TSagaState, TSagaActivity> : ISagaStep
        where TSagaState : ISagaState
        where TSagaActivity : ISagaActivity<TSagaState>
    {
        private readonly IServiceProvider serviceProvider;

        public String StepName { get; private set; }
        public bool Async { get; }

        public SagaStepForActivity(
            String StepName, IServiceProvider serviceProvider, Boolean async)
        {
            this.StepName = StepName;
            this.serviceProvider = serviceProvider;
            Async = async;
        }

        public async Task Run(IExecutionContext context, IEvent @event)
        {
            IExecutionContext<TSagaState> contextForAction =
                (IExecutionContext<TSagaState>)context;

            TSagaActivity activity = (TSagaActivity)ActivatorUtilities.
                CreateInstance(serviceProvider, typeof(TSagaActivity));

            if (activity != null)            
                await activity.Execute(contextForAction);            
        }
    }

}