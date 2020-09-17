using System;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Tests.SagaTests.IfElseIfElseSaga
{
    public class IfElseIfElseSagaData : ISagaData
    {
        public IfElseIfElseSagaData()
        {

        }

        public Guid ID { get; set; }

        public int Condition { get; set; }

        public object Value1 { get; set; }
        public int SubCondition { get; internal set; }
    }
}
