using System;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.Tests.SagaTests.ChildStepsSaga
{
    public class ChildStepsSagaData : ISagaData
    {
        public ChildStepsSagaData()
        {

        }

        public Guid ID { get; set; }
    }
}
