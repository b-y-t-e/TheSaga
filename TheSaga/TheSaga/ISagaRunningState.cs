using System;

namespace TheSaga
{
    public interface ISagaRunningState
    {
        public ESataStateAction RunningState { get; }
    }

    public enum ESataStateAction
    {
        WAITING,
        RUNNING_ACTIVITY,
        RUNNING_COMPENSATE
    }
}