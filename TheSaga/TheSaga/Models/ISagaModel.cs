using System;

namespace TheSaga.Models
{
    public interface ISagaModel
    {
        Type SagaStateType { get; }
        bool IsStartEvent(Type type);
        bool ContainsEvent(Type type);
    }
}