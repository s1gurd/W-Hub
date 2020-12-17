using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Common
{
    [NetworkSimObject]
    public class TimerComponent : MonoBehaviour
    {
        [NetworkSimData]
        public List<TimerAction> TimedActions = new List<TimerAction>();
        
        [NetworkSimData]
        public IActor actor;

        private void Awake()
        {
            actor = this.gameObject.GetComponent<IActor>();
            if (actor == null)
            {
                Debug.LogError("[TIMER COMPONENT] No IActor component found, aborting!");
                return;
            }

            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            dstManager.AddComponent<TimerData>(actor.ActorEntity);
        }

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

        [Button(ButtonSizes.Medium)]
        private void StopTimers()
        {
            var t = this.gameObject.GetComponent<ITimer>();
            t?.FinishTimer();
        }

        [Button(ButtonSizes.Medium)]
        private void StartTimers()
        {
            var t = this.gameObject.GetComponent<ITimer>();
            t?.StartTimer();
        }

#endif
    }

    public struct TimerAction
    {
        public Action Act;
        public float Delay;
    }

    public struct TimerActionEditor
    {
        public string Act;
        public float Delay;
    }
}