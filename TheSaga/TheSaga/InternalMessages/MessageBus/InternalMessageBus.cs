using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheSaga.Coordinators;
using TheSaga.Utils;

namespace TheSaga.InternalMessages.MessageBus
{
    public class InternalMessageBus : IInternalMessageBus
    {
        Dictionary<Type, Dictionary<Object, InternalMessageBus.Subscriber>> typesAndSubscribers =
            new Dictionary<Type, Dictionary<object, Subscriber>>();

        public void Publish(IInternalMessage message)
        {
            Type incomingMessageType = message.GetType();

            KeyValuePair<Type, Dictionary<object, Subscriber>>[] typesAndSubscribersArr = null;

            lock (typesAndSubscribers)
                typesAndSubscribersArr = typesAndSubscribers.ToArray();

            foreach (var typesAndSubs in typesAndSubscribersArr)
            {
                Type type = typesAndSubs.Key;
                if (incomingMessageType.Is(type))
                {
                    KeyValuePair<object, Subscriber>[] subscribersArr = null;

                    lock (typesAndSubscribers)
                        subscribersArr = typesAndSubs.Value.ToArray();

                    foreach (KeyValuePair<object, Subscriber> subscriber in subscribersArr)
                    {
                        Func<IInternalMessage, Task>[] actionsArr = null;

                        lock (typesAndSubscribers)
                            actionsArr = subscriber.Value.Actions.ToArray();

                        foreach (Func<IInternalMessage, Task> action in actionsArr)
                            action(message);
                    }
                }
            }

        }

        public void Subscribe<T>(object listener, Func<T, Task> handler)
        {
            lock (typesAndSubscribers)
            {
                Dictionary<object, Subscriber> dict = null;
                typesAndSubscribers.TryGetValue(typeof(T), out dict);
                if (dict == null)
                    dict = typesAndSubscribers[typeof(T)] = new Dictionary<object, Subscriber>();

                Subscriber subscriber = null;
                dict.TryGetValue(listener, out subscriber);
                if (subscriber == null)
                    subscriber = dict[listener] = new Subscriber()
                    {
                        Sub = subscriber
                    };

                Func<IInternalMessage, Task> func = (msg) => handler((T)msg);
                subscriber.Actions.Add(func);
            }
        }

        public void Unsubscribe<T>(object listener)
        {
            lock (typesAndSubscribers)
            {
                Dictionary<object, Subscriber> dict = null;
                typesAndSubscribers.TryGetValue(typeof(T), out dict);
                if (dict == null)
                    return;

                Subscriber subscriber = null;
                dict.TryGetValue(listener, out subscriber);
                if (subscriber == null)
                    return;

                subscriber.Actions.Clear();
                subscriber.Sub = null;
                dict.Remove(listener);
            }
        }

        internal class Subscriber
        {
            internal Object Sub;

            internal List<Func<IInternalMessage, Task>> Actions;

            public Subscriber()
            {
                Actions = new List<Func<IInternalMessage, Task>>();
            }
        }
    }
}
