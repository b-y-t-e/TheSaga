using System;
using System.Collections.Generic;
using System.Text;

namespace TheSaga.States
{
    internal static class Extensions
    {
        internal static string GetStateName<TState>()
            where TState : IState, new()
        {
            if (typeof(TState) == typeof(IStateWithCustomName) || typeof(IStateWithCustomName).IsAssignableFrom(typeof(TState)))
                return ((IStateWithCustomName)new TState()).Name;
            return nameof(TState);
        }

        static string GetStateName<TState>(TState state)
            where TState : IState
        {
            if (typeof(TState) == typeof(IStateWithCustomName) || typeof(IStateWithCustomName).IsAssignableFrom(typeof(TState)))
                return ((IStateWithCustomName)state).Name;
            return nameof(TState);
        }
    }
}
