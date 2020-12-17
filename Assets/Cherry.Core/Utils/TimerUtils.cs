using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using UnityEngine;

namespace GameFramework.Example.Utils
{
    public static class TimerUtils
    {
        public static void AddAction(this List<TimerAction> timedActions, Action action, float delay)
        {
            timedActions.Add(new TimerAction {Act = action, Delay = delay});
        }

        public static void AddAction(this TimerBaseBehaviour obj, Action action, float delay)
        {
            obj.Timer.TimedActions.Add(new TimerAction {Act = action, Delay = delay});
        }

        public static void AddAction(this TimerBaseBehaviour obj, Action action, Func<float> delay)
        {
            obj.Timer.TimedActions.Add(new TimerAction {Act = action, Delay = delay()});
        }

        public static void AddRepeatedAction(this TimerBaseBehaviour obj, Action action, Func<float> delay)
        {
            var d = delay();
            obj.Timer.TimedActions.Add(new TimerAction {Act = action, Delay = d});
            obj.Timer.TimedActions.Add(new TimerAction
            {
                Act = () =>
                {
                    if (obj.TimerActive) obj.AddRepeatedAction(action, delay);
                },
                Delay = d
            });
        }

        public static void RemoveAction(this TimerBaseBehaviour obj, Action action, bool onlyNext = false)
        {
            for (var i = 0; i < obj.Timer.TimedActions.Count; i++)
            {
                if (!obj.Timer.TimedActions[i].Act.Equals(action)) continue;
                
                obj.Timer.TimedActions.RemoveAt(i);
                i--;
                
                if (onlyNext) return;
            }
        }
        
        public static bool ContainsAction(this TimerBaseBehaviour obj, Action action)
        {
            return obj.Timer.TimedActions.Any(act => act.Act.Equals(action));
        }
        
        public static TimerAction? GetCurrentTimer(this TimerBaseBehaviour obj)
        {
            if (obj.Timer == null) return null;
            if (!obj.Timer.TimedActions.Any()) return null;

            return obj.Timer.TimedActions.First();
        }

        public static TimerComponent GetOrCreateTimer(this GameObject obj, TimerComponent timer)
        {
            if (timer != null) return timer;
            return (timer = obj.GetComponent<TimerComponent>()) != null ? timer : obj.AddComponent<TimerComponent>();
        }
    }
}