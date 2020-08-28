using System;
using System.Threading.Tasks;

namespace TheSaga.Messages.MessageBus
{
    public interface IInternalMessageBus
    {
        Task Publish(IInternalMessage message);

        void Subscribe<T>(object listener, Func<T, Task> p);

        void Unsubscribe<T>(object listener);
    }
}