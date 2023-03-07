using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using System;

namespace LR
{
    public class EventDispatcher : MonoBehaviour
    {
        private static EventDispatcher _instance;
        public static EventDispatcher Instance {
            get
            {
                if(_instance == null)
                {
                    var go = new GameObject("EventDispatcher");
                    _instance = go.AddComponent<EventDispatcher>();
                }
                return _instance;
            }
        }

        private readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();
        private readonly Pool<List<Delegate>> _subscriberListPool = new Pool<List<Delegate>>(() => new List<Delegate>(), list => list.Clear());

        #region Subscribe

        public void Subscribe<T>(Action<T> callback) where T : struct
        {
            AddToSubscriptionList(typeof(T), callback);
        }

        public void Subscribe(Type eventType, Action callback)
        {
            AddToSubscriptionList(eventType, callback);
        }


        private void AddToSubscriptionList(Type eventType, Delegate callback)
        {
            if (!_subscribers.TryGetValue(eventType, out List<Delegate> subscriberList))
                _subscribers[eventType] = subscriberList = new List<Delegate>(1);
            subscriberList.Add(callback);
        }

        #endregion

        #region Unsubscribe

        public void Unsubscribe<T>(Action<T> callback) where T : struct
        {
            Type eventType = typeof(T);
            if (_subscribers.TryGetValue(eventType, out List<Delegate> subscriberList))
                subscriberList.Remove(callback);
        }

        public void Unsubscribe<T>(Action callback) where T : struct
        {
            Unsubscribe(typeof(T), callback);
        }

        public void Unsubscribe(Type eventType, Action callback)
        {
            if (_subscribers.TryGetValue(eventType, out List<Delegate> subscriberList))
                subscriberList.Remove(callback);
        }

        #endregion

        #region Publish

        public void Publish<T>() where T : struct
        {
            Publish(default(T));
        }

        public void Publish<T>(T @event) where T : struct
        {
            Type eventType = @event.GetType();

            bool foundListener = false;
            if (_subscribers.TryGetValue(eventType, out List<Delegate> subscribers))
            {
                List<Delegate> subscriberList = _subscriberListPool.Take();
                try
                {
                    subscriberList.AddRange(subscribers);

                    foreach(Delegate callback in subscriberList)
                    {
                        switch (callback)
                        {
                            case Action<T> typedCallback:
                                typedCallback(@event);
                                foundListener = true;
                                break;
                            case Action parameterlessCallback:
                                parameterlessCallback();
                                foundListener = true;
                                break;
                            default:
                                Debug.LogError($"[EventDispatcher] Invalid event subscriber type : {callback.GetType()}");
                                continue;
                        }
                    }
                }
                finally
                {
                    _subscriberListPool.Return(subscriberList);
                }

                if (!foundListener)
                    Debug.LogWarning($"[EventDispatcher] No listener for published event : {@event}");
            }
        }
        #endregion

    }
}