namespace RoomBuildingStarterKit.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    /// <summary>
    /// The EventManager class.
    /// </summary>
    public class EventManager
    {
        /// <summary>
        /// The Event definitions.
        /// </summary>
        public enum Event
        {
            Save,
            UIBlockMouse,
        }

        /// <summary>
        /// The EventInfo struct.
        /// </summary>
        public class Info
        {
            /// <summary>
            /// The observer object.
            /// </summary>
            public object obj;

            /// <summary>
            /// The method name to be called.
            /// </summary>
            public string methodName;

            /// <summary>
            /// The method info to be called.
            /// </summary>
            public MethodInfo method;

            /// <summary>
            /// 
            /// </summary>
            public bool isDelete;
        };

        /// <summary>
        /// The events.
        /// </summary>
        private static Dictionary<Event, List<Info>> events = new Dictionary<Event, List<Info>>();

        /// <summary>
        /// Registers events.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="obj">The observer.</param>
        /// <param name="methodName">The method name.</param>
        /// <returns>Registers success or not.</returns>
        public static bool RegisterEvent(Event eventName, object obj, string methodName)
        {
            EventManager.UnRegisterEvent(eventName, obj, methodName);
            EventManager.RecycleEvents(eventName);

            List<Info> eventInfos = null;
            var info = new Info { obj = obj, method = obj.GetType().GetMethod(methodName), methodName = methodName, isDelete = false };

            if (info.method == null)
            {
                Debug.LogError($"Event::register: {obj} not found method[{methodName}]");
                return false;
            }

            // Debug.Log($"RegisterEvent {eventName}, ClassName {obj.GetType().ToString()}, MethodName {methodName}");

            if (!events.TryGetValue(eventName, out eventInfos))
            {
                eventInfos = new List<Info>();
                eventInfos.Add(info);
                events.Add(eventName, eventInfos);
                return true;
            }

            eventInfos.Add(info);
            return true;
        }

        /// <summary>
        /// UnRegisters events.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="obj">The observer.</param>
        /// <param name="methodName">The method name.</param>
        /// <returns>UnRegisters success or not.</returns>
        public static bool UnRegisterEvent(Event eventName, object obj, string methodName)
        {
            List<Info> eventInfos = null;
            if (!events.TryGetValue(eventName, out eventInfos))
            {
                return false;
            }

            for (int i = 0; i < eventInfos.Count; ++i)
            {
                if (obj == eventInfos[i].obj && eventInfos[i].methodName == methodName)
                {
                    eventInfos[i].isDelete = true;
                    
                    // Debug.Log($"UnRegisterEvent {eventName}, ClassName {obj.GetType().ToString()}, MethodName {methodName}");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Triggers event.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        /// <param name="args">The method arguments.</param>
        public static void TriggerEvent(Event eventName, params object[] args)
        {
            // Debug.Log($"TriggerEvent {eventName}");
            EventManager.RecycleEvents(eventName);

            if (!events.TryGetValue(eventName, out List<Info> eventInfos))
            {
                return;
            }

            for (int i = 0; i < eventInfos.Count; ++i)
            {
                var info = eventInfos[i];

                try
                {
                    info.method.Invoke(info.obj, args);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"ClassName: {info.obj.GetType().ToString()}, MethodName: {info.methodName}, exception: {ex}");
                }
            }
        }

        /// <summary>
        /// Recycles events.
        /// </summary>
        /// <param name="eventName">The event name.</param>
        private static void RecycleEvents(Event eventName)
        {
            if (events.TryGetValue(eventName, out List<Info> eventInfos))
            {
                events[eventName] = eventInfos.Where(info => info.isDelete == false && info.obj != null).ToList();
            }
        }
    }
}
