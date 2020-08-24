using System.Threading.Tasks;
using TheSaga.Interfaces;
using TheSaga.States;

namespace TheSaga.Instances
{
    public interface ISagaInstance
    {
        ISagaState State { get; }
        Task Push(IEvent @event);
    }
}