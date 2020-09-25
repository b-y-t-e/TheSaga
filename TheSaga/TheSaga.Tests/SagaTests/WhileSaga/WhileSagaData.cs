using System;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Tests.SagaTests.WhileSaga
{
    public class WhileSagaData : ISagaData
    {
        public WhileSagaData()
        {

        }

        public Guid ID { get; set; }

        public int Counter { get; set; }
        public int Counter2 { get; set; }
        public int Value { get; set; }
        public int SecondValue { get; set; }
    }
}
