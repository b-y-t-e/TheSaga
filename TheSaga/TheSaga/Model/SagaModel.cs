using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TheSaga.Model
{
    public class SagaModel
    {
        public Type SagaType { get; private set; }

        public List<String> States { get; private set; }

        public List<Type> Events { get; private set; }

        public SagaModel()
        {
            States = new List<string>();
            Events = new List<Type>();
        }



        public void Init(Type sagaType)
        {
            this.SagaType = sagaType;

            this.States.AddRange(
                sagaType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).
                Where(p => p.PropertyType == typeof(IState) || p.PropertyType.IsAssignableFrom(typeof(IState))).
                Select(p => p.Name));

            this.States.AddRange(
                sagaType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).
                Where(p => p.FieldType == typeof(IState) || p.FieldType.IsAssignableFrom(typeof(IState))).
                Select(p => p.Name));

            this.Events.AddRange(
                sagaType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).
                Where(p => p.PropertyType == typeof(IEvent) || p.PropertyType.IsAssignableFrom(typeof(IEvent))).
                Select(p => p.PropertyType));

            this.Events.AddRange(
                sagaType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).
                Where(p => p.FieldType == typeof(IEvent) || p.FieldType.IsAssignableFrom(typeof(IEvent))).
                Select(p => p.FieldType));
        }



    }
}