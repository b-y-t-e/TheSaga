using TheSaga.Utils;

namespace TheSaga.States
{
    internal static class Extensions
    {
        internal static string GetStateName<TState>(this TState state)
            where TState : IState
        {
            if (typeof(TState).Is<IStateWithCustomName>())
                return ((IStateWithCustomName) state).Name;

            return typeof(TState).Name;
        }
    }
}