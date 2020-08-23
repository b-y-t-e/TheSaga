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

            var sagaBuilder = new SagaBuilder<OrderTestSaga, OrderState>();
            sagaBuilder.Start(new Utworzone());

           /*ISagaRegistrator sagaRegistrator = new SagaRegistrator();
            sagaRegistrator.Register<OrderTestSaga, OrderState>("order-test");

            ISagaCoordinator sagaCoordinator = new SagaCoordinator();
            sagaCoordinator.Execute(new Utworzone());
           */
        }
    }

    public class OrderState : ISagaState
    {
        public Guid CorrelationID { get; set; }
        public string CurrentState { get; set; }
        public string CurrentActivity { get; set; }
    }

    public class OrderTestSaga : ISaga<OrderState>
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

        public void Define<TSagaType>(ISagaBuilder<TSagaType, OrderState> builder) where TSagaType : ISaga<OrderState>
        {
            builder.
                Start(Utworzone).
                Then(ctx => { }).
                TransitionTo(Nowe);

            builder.
                During(Nowe).
                When(Skompletowano).
                Then(typeof(WyslijEmailDoKlienta)).
                Then(typeof(WyslijWiadomoscDoKierownika)).
                Then(typeof(ZamowKuriera)).
                TransitionTo(Skompletowane);

            builder.
                During(Skompletowane).
                When(Wyslano).
                Then(ctx => { }).
                TransitionTo(Wyslane);

            builder.
                During(Wyslane).
                When(Dostarczono).
                Then(ctx => { }).
                TransitionTo(Zakonczono);

            builder.
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
