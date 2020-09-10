using System;
using TheSaga.Models;

namespace TheSaga.Tests.SagaTests.Sagas.IfElseSaga
{
    public class IfElseSagaData : ISagaData
    {
        public IfElseSagaData()
        {

        }

        public Guid ID { get; set; }

        public Boolean Condition { get; set; }

        public object Value1 { get; set; }
    }
}