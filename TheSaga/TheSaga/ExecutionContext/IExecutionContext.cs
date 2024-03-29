﻿using System.Threading.Tasks;
using TheSaga.Models;
using TheSaga.Models.Interfaces;

namespace TheSaga.ExecutionContext
{
    public interface IExecutionContext<TSagaData> : IExecutionContext
        where TSagaData : ISagaData
    {
        TSagaData Data { get; }
    }

    public interface IExecutionContext
    {
        ISagaExecutionInfo ExecutionInfo { get; }

        ISagaExecutionState ExecutionState { get; }

        ISagaExecutionValues ExecutionValues { get; }

        IStepExecutionValues StepExecutionValues { get; }

        internal Task Stop();
    }
}
