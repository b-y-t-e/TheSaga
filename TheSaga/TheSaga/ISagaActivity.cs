using System;
using System.Threading.Tasks;

namespace TheSaga
{
    public interface ISagaActivity<TSagaState>
            where TSagaState : ISagaState
    {
        Task Execute(IContext<TSagaState> context);

        Task Compensate(IContext<TSagaState> context);
    }
}