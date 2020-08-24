using System.Threading.Tasks;

namespace TheSaga
{
    public interface ISagaInstance
    {
        Task Push(IEvent @event);
    }
}