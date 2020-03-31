using System;
using System.Collections.Generic;
using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Components.Interfaces;
using UnityEngine;

namespace GameFramework.Example.Utils
{
    public static class TimerUtils
    {
        public static void AddAction(this List<TimerAction> timedActions, Action action, float delay)
        {
            timedActions.Add(new TimerAction {Act = action, Delay = delay});
        }

        public static TimerComponent GetOrCreateTimer(this GameObject obj, TimerComponent timer)
        {
            if (timer != null) return timer;
            return (timer = obj.GetComponent<TimerComponent>()) != null ? timer : obj.AddComponent<TimerComponent>();
        }
    }
}