using System;
using System.Collections.Generic;
using System.Reflection;

namespace DynamicControllersGeneration
{
    internal class EventsRaiser
    {
        private const BindingFlags EVENT_BACKING_FIELD_FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;

        private readonly object instance;

        public EventsRaiser(object instance)
        {
            this.instance = instance;
        }

        public void Raise(string eventName, params object[] arguments)
        {
            IEnumerable<Delegate> invocationList = GetInvocationList(instance, eventName);
            InvokeDelegates(invocationList, arguments);
        }

        private static IEnumerable<Delegate> GetInvocationList(object instance, string eventName)
        {
            Type typeDeclaringEvent = GetTypeDeclaringEvent(instance.GetType(), eventName);
            MulticastDelegate eventDelegate = GetEventDelegate(instance, eventName, typeDeclaringEvent);
            return eventDelegate.GetInvocationList();
        }

        private static MulticastDelegate GetEventDelegate(object instance, string eventName, Type typeDeclaringEvent)
        {
            var eventBackingField = typeDeclaringEvent.GetField(eventName, EVENT_BACKING_FIELD_FLAGS);
            return (MulticastDelegate)eventBackingField.GetValue(instance);
        }

        private static Type GetTypeDeclaringEvent(Type instanceType, string eventName)
        {
            return instanceType.GetEvent(eventName).DeclaringType;
        }

        private static void InvokeDelegates(IEnumerable<Delegate> invocationList, object[] arguments)
        {
            foreach (var invocation in invocationList)
            {
                invocation.DynamicInvoke(arguments);
            }
        }
    }
}