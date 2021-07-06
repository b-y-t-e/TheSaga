
namespace TheSaga.Models
{
    public interface ISagaRunningState
    {
        bool IsRunning { get; set; }
        bool IsCompensating { get; set; }
        bool IsResuming { get; set; }
    }
}