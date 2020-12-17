using System;
using System.Collections.Generic;
using GameFramework.Example.AI;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityAIInput : TimerBaseBehaviour, IActorAbility, IAIModule
    {
        public IActor Actor { get; set; }

        public List<AIBehaviourSetting> behaviours;

        [MinMaxSlider(0, 30, true)] public Vector2 behaviourUpdatePeriod = new Vector2(2f, 6f);

        [HideInInspector] public AIBehaviourSetting activeBehaviour;
        [HideInInspector] public float activeBehaviourPriority = 0;

        private Entity _entity;
        private EntityManager _dstManager;

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
            _entity = entity;
            _dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            _dstManager.AddComponent<NetworkSyncReceive>(entity);

            for (var i = 0; i < behaviours.Count; i++)
            {
                behaviours[i] = behaviours[i].CopyBehaviour();
                behaviours[i].Actor = Actor;
            }

            StartTimer();
            EvaluateAll();
        }

        private void Start()
        {
            var tempBehaviours = new List<AIBehaviourSetting>();

            foreach (var t in behaviours)
            {
                tempBehaviours.Add(t.CopyBehaviour());
            }

            behaviours = tempBehaviours;
        }

        public void EvaluateAll()
        {
            this.RemoveAction(EvaluateAll);

            _dstManager.RemoveComponent<SetupAIData>(_entity);
            _dstManager.AddComponent<EvaluateAIData>(_entity);

            if (this.TimerActive)
            {
                this.AddAction(EvaluateAll, Random.Range(behaviourUpdatePeriod[0], behaviourUpdatePeriod[1]));
            }
        }

        public void Execute()
        {
        }
    }

    public struct EvaluateAIData : IComponentData
    {
    }

    public struct SetupAIData : IComponentData
    {
    }
}