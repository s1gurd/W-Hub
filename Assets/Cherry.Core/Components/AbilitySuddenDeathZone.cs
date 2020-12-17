using System;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [RequireComponent(typeof(SphereCollider))]
    public class AbilitySuddenDeathZone : TimerBaseBehaviour, IActorAbility
    {
        public Transform deathZoneTransform;
        
        public float initialDelay;
        public float reductionDelay = 1;
        
        public float speedOfRadiusReduction;
        public float minimumRadius;
        
        [TitleGroup("Outside Zone Players Settings")]
        public float healthDecrement = 20;
        public float applyPeriod = 2;

        [TitleGroup("")]
        public bool executeOnStart;

        public IActor Actor { get; set; }

        public SphereCollider ZoneCollider
        {
            get
            {
                if (_collider != null) return _collider;
                
                _collider = gameObject.GetComponent<SphereCollider>();
                return _collider;
            }
        }

        private SphereCollider _collider;
        private Vector3 _scaleCoefficient;

        private EntityManager _dstManager;

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
            
            _dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            if (executeOnStart) Execute();
        }

        public void Execute()
        {
            if (Actor == null || ZoneCollider == null) return;

            if (Math.Abs(initialDelay) < 0.01f)
            {
                ReduceDeathZoneRadius();
                _dstManager.AddComponent<ApplySuddenDeathData>(Actor.ActorEntity);
                return;
            }
            
            if (Timer == null) return;

            _scaleCoefficient = deathZoneTransform.localScale / ZoneCollider.radius;

            Timer.TimedActions.AddAction(() =>
            {
                ReduceDeathZoneRadius();
                _dstManager.AddComponent<ApplySuddenDeathData>(Actor.ActorEntity);
            }, initialDelay);
        }

        private void ReduceDeathZoneRadius()
        {
            var colliderRadius = ZoneCollider.radius;
            if (colliderRadius <= minimumRadius) return;

            colliderRadius -= speedOfRadiusReduction;
            deathZoneTransform.localScale = _scaleCoefficient * colliderRadius;

            ZoneCollider.radius = colliderRadius;
            
            Timer.TimedActions.AddAction(ReduceDeathZoneRadius, reductionDelay);
        }
    }

    public struct ApplySuddenDeathData : IComponentData
    {
    }
}