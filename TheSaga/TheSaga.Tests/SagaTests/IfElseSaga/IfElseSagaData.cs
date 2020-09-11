using System;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Tests.SagaTests.IfElseSaga
{
    public class IfElseSagaData : ISagaData
    {
        public IfElseSagaData()
        {

        }

        public Guid ID { get; set; }

        public int Condition { get; set; }

        public object Value1 { get; set; }
    }
}
