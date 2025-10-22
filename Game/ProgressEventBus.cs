using System;
using System.Collections.Generic;

namespace conscious
{
    public static class EventBus
    {
        // Use a dictionary to store event handlers for each event type
        private static readonly Dictionary<Type, Delegate> _events = new();

        public static void Subscribe<TEventArgs>(EventHandler<TEventArgs> handler)
            where TEventArgs : EventArgs
        {
            var eventType = typeof(TEventArgs);
            if (!_events.TryGetValue(eventType, out var existingHandlers))
            {
                _events[eventType] = handler;
            }
            else
            {
                _events[eventType] = Delegate.Combine(existingHandlers, handler);
            }
        }

        public static void Unsubscribe<TEventArgs>(EventHandler<TEventArgs> handler)
            where TEventArgs : EventArgs
        {
            var eventType = typeof(TEventArgs);
            if (_events.TryGetValue(eventType, out var existingHandlers))
            {
                _events[eventType] = Delegate.Remove(existingHandlers, handler);
            }
        }

        public static void Publish<TEventArgs>(object sender, TEventArgs e)
            where TEventArgs : EventArgs
        {
            var eventType = typeof(TEventArgs);
            if (_events.TryGetValue(eventType, out var handlers))
            {
                ((EventHandler<TEventArgs>)handlers)?.Invoke(sender, e);
            }
        }
    }
}