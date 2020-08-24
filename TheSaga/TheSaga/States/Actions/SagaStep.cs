using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;
using TheSaga.Interfaces;

namespace TheSaga.States.Actions
{
    public class SagaStep<TSagaState> : ISagaStep
        where TSagaState : ISagaState
    {
        public String StepName { get; private set; }

        public ThenActionDelegate<TSagaState> Action { get; private set; }

        public Type Activity { get; private set; }

        public SagaStep(String StepName, ThenActionDelegate<TSagaState> Action)
        {
            this.StepName = StepName;
            this.Action = Action;
        }

        public SagaStep(String StepName, Type Activity)
        {
            this.StepName = StepName;
            this.Activity = Activity;
        }

        public async Task Execute(IInstanceContext context, IEvent @event)
        {
            if (Action != null)
            {
                await Action((IInstanceContext<TSagaState>)context);
            }
            else if (Activity != null)
            {
                var activity = Activator.CreateInstance(Activity);

                var method = activity.GetType().GetMethod("Execute",
                    BindingFlags.Public | BindingFlags.Instance);

                throw new NotImplementedException();
            }
        }
    }

    public delegate Task ThenActionDelegate<TSagaState>(IInstanceContext<TSagaState> context) 
        where TSagaState : ISagaState;
}