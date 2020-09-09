using System;
using TheSaga.Models;

namespace TheSaga.Tests.SagaTests.Sagas.ChildStepsSaga
{
    public class ChildStepsSagaData : ISagaData
    {
        public ChildStepsSagaData()
        {

        }

        public Guid ID { get; set; }
    }
}