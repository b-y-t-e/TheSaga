using System;
using System.Threading;
using Xunit;
using Xunit.Sdk;

namespace TheSaga.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            OrderTestSaga testSaga = new OrderTestSaga();

            ISagaRegistrator sagaRegistrator = new SagaRegistrator();
            sagaRegistrator.Register(testSaga);

            ISagaCoordinator sagaCoordinator = new SagaCoordinator();
            sagaCoordinator.Send(new Utworzone());
        }
    }

    public class OrderData : ISagaState
    {

    }

    public class OrderTestSaga : ISaga<OrderData>
    {
        IState Nowe;

        IState Skompletowane;

        IState Wyslane;

        IState Dostarczone;

        IState Zakonczono;

        //////////////////////////////////

        IEvent Utworzone;

        IEvent Skompletowano;

        IEvent Wyslano;

        IEvent Dostarczono;

        //////////////////////////////////

        public OrderTestSaga()
        {
            this.
                Start(Utworzone).
                Then(ctx => { }).
                TransitionTo(Nowe);

            this.
                During(Nowe).
                When(Skompletowano).
                Then(typeof(WyslijEmailDoKlienta)).
                Then(typeof(WyslijWiadomoscDoKierownika)).
                Then(typeof(ZamowKuriera)).
                TransitionTo(Skompletowane);

            this.
                During(Skompletowane).
                When(Wyslano).
                Then(ctx => { }).
                TransitionTo(Wyslane);

            this.
                During(Wyslane).
                When(Dostarczono).
                Then(ctx => { }).
                TransitionTo(Zakonczono);

            this.
                During(Wyslane).
                After(TimeSpan.FromDays(30)).
                Then(ctx => { }).
                TransitionTo(Zakonczono);
        }
    }

    internal class ZamowKuriera : ISagaActivity
    {
    }

    internal class WyslijWiadomoscDoKierownika : ISagaActivity
    {
    }

    internal class WyslijEmailDoKlienta : ISagaActivity
    {
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
