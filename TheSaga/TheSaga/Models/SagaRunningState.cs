
namespace TheSaga.Models
{
    public class SagaRunningState : ISagaRunningState
    {
        public bool IsRunning { get; set; }
        public bool IsCompensating { get; set; }
        public bool IsResuming { get; set; }
    }
}