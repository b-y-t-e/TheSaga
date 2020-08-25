using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheSaga.Coordinators;

namespace TheSaga.Messages.MessageBus
{
    public interface IInternalMessageBus
    {
        void Publish(IInternalMessage message);
        void Subscribe<T>(object listener, Func<T, Task> p);
        void Unsubscribe<T>(object listener);
    }
}
