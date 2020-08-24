using System;
using System.Threading.Tasks;
using TheSaga.Activities;
using TheSaga.Builders;

namespace TheSaga.Tests.Sagas.OrderTestSaga.Activities
{
    internal class WyslijEmailDoKlienta : ISagaActivity<OrderState>
    {
        public Task Compensate(IInstanceContext<OrderState> context)
        {
            throw new NotImplementedException();
        }

        public Task Execute(IInstanceContext<OrderState> context)
        {
            throw new NotImplementedException();
        }
    }
}
