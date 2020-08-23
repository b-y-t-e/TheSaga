using System;
using System.Threading;
using System.Threading.Tasks;
using TheSaga.Model;
using Xunit;
using Xunit.Sdk;

namespace TheSaga.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var sagaModel = OrderTestSaga.Create();
            sagaModel = sagaModel;

            ISagaRegistrator sagaRegistrator = new SagaRegistrator();
            sagaRegistrator.Register("order-test", sagaModel);

            ISagaSeeker sagaSeeker = new InMemorySagaSeeker();

            ISagaCoordinator sagaCoordinator = new SagaCoordinator(sagaSeeker);
            sagaCoordinator.Execute(new Utworzone());
        }
    }

    public class OrderState : ISagaState
    {
        public Guid CorrelationID { get; set; }
        public string CurrentState { get; set; }
        public string CurrentActivity { get; set; }
    }

    public static class OrderTestSaga
    {
        public static SagaModel<OrderState> Create()
        {
            SagaBuilder<OrderState> builder = new SagaBuilder<OrderState>();

            builder.
                Start<Utworzone>().
                Then(async ctx => { }).
                TransitionTo<Nowe>();

            builder.
                During<Nowe>().
                When<Skompletowano>().
                Then<WyslijEmailDoKlienta>().
                Then<WyslijWiadomoscDoKierownika>().
                Then<ZamowKuriera>().
                TransitionTo<Skompletowane>();

            builder.
                During<Skompletowane>().
                When<Wyslano>().
                Then(async ctx => { }).
                TransitionTo<Wyslane>();

            builder.
                During<Wyslane>().
                When<Dostarczono>().
                Then(async ctx => { }).
                TransitionTo<Zakonczono>();

            builder.
                During<Wyslane>().
                After(TimeSpan.FromDays(30)).
                Then(async ctx => { }).
                TransitionTo<Zakonczono>();

            return builder.
                Build();
        }
    }

    internal class Nowe : IState { }

    internal class Skompletowane : IState { }

    internal class Wyslane : IState { }

    internal class Dostarczone : IState { }

    internal class Zakonczono : IState { }


    internal class ZamowKuriera : ISagaActivity<OrderState>
    {
        public Task Compensate(IContext<OrderState> context)
        {
            throw new NotImplementedException();
        }

        public Task Execute(IContext<OrderState> context)
        {
            throw new NotImplementedException();
        }
    }

    internal class WyslijWiadomoscDoKierownika : ISagaActivity<OrderState>
    {
        public Task Compensate(IContext<OrderState> context)
        {
            throw new NotImplementedException();
        }

        public Task Execute(IContext<OrderState> context)
        {
            throw new NotImplementedException();
        }
    }

    internal class WyslijEmailDoKlienta : ISagaActivity<OrderState>
    {
        public Task Compensate(IContext<OrderState> context)
        {
            throw new NotImplementedException();
        }

        public Task Execute(IContext<OrderState> context)
        {
            throw new NotImplementedException();
        }
    }

    public class Dostarczono : IEvent
    {
        public Guid CorrelationID { get; set; }
    }

    public class Wyslano : IEvent
    {
        public Guid CorrelationID { get; set; }
    }

    public class Skompletowano : IEvent
    {
        public Guid CorrelationID { get; set; }
    }

    public class Utworzone : IEvent
    {
        public Guid CorrelationID { get; set; }
    }
}
