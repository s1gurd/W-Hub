using GameFramework.Example.Common;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityActorHealth : MonoBehaviour, IActorAbility, ITimer
    {
        public int maxHealth;
        [HideInInspector] public int health;
        public float corpseCleanupDelay;

        public ActorDeathAnimProperties actorDeathAnimProperties;
        public ActorTakeDamageAnimProperties actorTakeDamageAnimProperties;

        public bool TimerActive
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        public TimerComponent Timer =>
            _timer = this.gameObject.GetOrCreateTimer(_timer);

        [HideInInspector] public bool isEnabled = true;

        private TimerComponent _timer;
        private Entity _entity;

        public void AddComponentData(ref Entity entity)
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _entity = entity;
            
            dstManager.AddComponentData(entity, new ActorHealthData
            {
                MaxHealthValue = maxHealth,
                HealthValue = maxHealth
            });

            _timer = this.gameObject.GetOrCreateTimer(_timer);
            
            dstManager.AddComponent<TimerData>(entity);

            if (actorTakeDamageAnimProperties.HasActorTakeDamageAnimation)
            {
                dstManager.AddComponentData(entity, new ActorTakeDamageAnimData
                {
                    AnimHash = Animator.StringToHash(actorTakeDamageAnimProperties.ActorTakeDamageName)
                });
            }

            if (actorDeathAnimProperties.HasActorDeathAnimation)
            {
                dstManager.AddComponentData(entity, new ActorDeathAnimData
                {
                    AnimHash = Animator.StringToHash(actorDeathAnimProperties.ActorDeathAnimationName)
                });
            }
        }

        public void Execute()
        {
        }

        public void StartDeathTimer()
        {
            StartTimer();
        }

        public void FinishTimer()
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponent<ImmediateActorDestructionData>(_entity);
        }

        public void StartTimer()
        {
            Timer.TimedActions.AddAction(FinishTimer, corpseCleanupDelay);
        }
    }

    public struct ActorHealthData : IComponentData
    {
        public int HealthValue;
        public int MaxHealthValue;
    }

    public struct ActorDeathAnimData : IComponentData
    {
        public int AnimHash;
    }

    public struct ActorTakeDamageAnimData : IComponentData
    {
        public int AnimHash;
    }

    public struct DeadActorData : IComponentData
    {
    }
    
    public struct DamagedActorData : IComponentData
    {
    }

    public struct ImmediateActorDestructionData : IComponentData
    {
    }
}