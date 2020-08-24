using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.States;

namespace TheSaga.Activities
{
    public class IInMemoryActivity<TSagaState> : ISagaActivity<TSagaState>
            where TSagaState : ISagaState
    {
        Action<IInstanceContext<TSagaState>> execute;

        public IInMemoryActivity(Action<IInstanceContext<TSagaState>> execute)
        {
            this.execute = execute;
        }

        public async Task Compensate(IInstanceContext<TSagaState> context)
        {

        }

        public async Task Execute(IInstanceContext<TSagaState> context)
        {
            if (execute != null)
                execute(context);
        }
    }
}