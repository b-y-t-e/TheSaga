using System;
using System.Threading.Tasks;
using TheSaga.Builders;
using TheSaga.States;

namespace TheSaga.Activities
{
    public interface ISagaActivity<TSagaState>
            where TSagaState : ISagaState
    {
        Task Execute(IInstanceContext<TSagaState> context);

        Task Compensate(IInstanceContext<TSagaState> context);
    }
}