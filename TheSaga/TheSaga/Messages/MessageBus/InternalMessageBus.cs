using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheSaga.Coordinators;

namespace TheSaga.Messages.MessageBus
{
    public class InternalMessageBus : IInternalMessageBus
    {
        Dictionary<Type, Dictionary<Object, InternalMessageBus.Subscriber>> typesAndSubscribers =
            new Dictionary<Type, Dictionary<object, Subscriber>>();

        public void Publish(IInternalMessage message)
        {
            //Task.Run(() =>
            // {
            Type incomingMessageType = message.GetType();
            lock (typesAndSubscribers)
            {
                foreach (var typesAndSubs in typesAndSubscribers)
                {
                    Type type = typesAndSubs.Key;

                    if (type == incomingMessageType ||
                        type.IsAssignableFrom(incomingMessageType))
                    {
                        foreach (var typeAndSub in typesAndSubs.Value)
                        {
                            foreach (var action in typeAndSub.Value.Actions)
                            {
                                //try
                                //{/
                                    action(message);
                                //}
                               // catch (Exception ex)
                               // {
                                //    Console.WriteLine(ex.Message);
                               // }
                            }
                        }
                    }
                }
            }
            //});
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
