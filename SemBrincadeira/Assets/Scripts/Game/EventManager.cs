using System;
using UnityEngine;
using System.Collections.Generic;

namespace FPHorror.Gameplay
{
    public static class EventManager
    {
        private static Dictionary<Type, Action<object>> eventDictionary = new Dictionary<Type, Action<object>>();

        public static void AddListener<T>(Action<T> listener) where T : class
        {
            Action<object> wrappedListener = (obj) => listener(obj as T);

            if (eventDictionary.TryGetValue(typeof(T), out var action))
            {
                action += wrappedListener;
                eventDictionary[typeof(T)] = action;
            }
            else
            {
                eventDictionary[typeof(T)] = wrappedListener;
            }
        }

        public static void RemoveListener<T>(Action<T> listener) where T : class
        {
            Action<object> wrappedListener = (obj) => listener(obj as T);

            if (eventDictionary.TryGetValue(typeof(T), out var action))
            {
                action -= wrappedListener;
                if (action == null)
                {
                    eventDictionary.Remove(typeof(T));
                }
                else
                {
                    eventDictionary[typeof(T)] = action;
                }
            }
        }

        public static void Broadcast<T>(T eventArgs) where T : class
        {
            if (eventDictionary.TryGetValue(typeof(T), out var action))
            {
                action?.Invoke(eventArgs);
            }
        }
    }
}