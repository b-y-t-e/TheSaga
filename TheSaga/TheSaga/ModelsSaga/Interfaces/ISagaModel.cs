using System;
using System.Collections.Generic;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.Steps;

namespace TheSaga.SagaModels
{


    public interface ISagaModel
    {
        ISagaActions Actions { get; }
        Type SagaStateType { get; }
        string Name { get; set; }
    }
}