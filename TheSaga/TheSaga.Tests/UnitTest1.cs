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

    public class OrderTestSaga : ISaga
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
                Then(ctx => { }).
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
