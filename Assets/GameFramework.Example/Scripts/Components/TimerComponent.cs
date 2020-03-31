using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Components
{
    public class TimerComponent : MonoBehaviour
    {
        public List<TimerAction> TimedActions = new List<TimerAction>();

#if UNITY_EDITOR
        [ShowInInspector]
        private List<TimerActionEditor> Actions
        {
            get
            {
                return TimedActions.Select(action => new TimerActionEditor
                    {Act = action.Act.Method.Name, Delay = action.Delay}).ToList();
            }
        }
#endif
    }
}