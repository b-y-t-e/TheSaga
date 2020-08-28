using System;
using System.Collections.Generic;
using TheSaga.Models;

namespace TheSaga.Tests.Sagas.TransitionsSaga
{
    public class TransitionsSagaData : ISagaData
    {
        public TransitionsSagaData()
        {

        }

        public Guid ID { get; set; }
    }
}