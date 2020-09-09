using System;
using System.Collections.Generic;
using System.Linq;
using TheSaga.Models;
using TheSaga.SagaModels.Actions;
using TheSaga.SagaModels.Steps;
using TheSaga.States;

namespace TheSaga.SagaModels
{
    internal class SagaModel : ISagaModel
    {
        public ISagaActions Actions { get; }

        public SagaModel(Type SagaStateType)
        {
            this.Name = $"{SagaStateType.Name}Model";
            this.SagaStateType = SagaStateType;
            Actions = new SagaActions();
        }

        public string Name { get; set; }
        public Type SagaStateType { get; }
    }
}