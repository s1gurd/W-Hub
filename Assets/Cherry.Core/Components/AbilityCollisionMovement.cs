using System;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityCollisionMovement : MonoBehaviour, IActorAbility
    {
        public IActor Actor { get; set; }

        public CollisionMovementSettings collisionMovementSettings;
        
        private Entity _entity;
        public void AddComponentData(ref Entity entity, IActor actor)
        {
            _entity = entity;
            Actor = actor;
        }

        public void Execute()
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            switch (collisionMovementSettings.reaction)
            {
                case CollisionMovementReaction.Ignore:
                    break;
                case CollisionMovementReaction.Stop:
                    dstManager.AddComponent<StopMovementData>(_entity);
                    if (collisionMovementSettings.alsoStopRotation) dstManager.AddComponent<StopRotationData>(_entity);
                    break;
                case CollisionMovementReaction.Bounce:
                    dstManager.AddComponentData(_entity, new BounceData
                    {
                        Force2DBounce = collisionMovementSettings.force2DBounce
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Serializable]
    public class CollisionMovementSettings
    {
        [EnumToggleButtons]
        public CollisionMovementReaction reaction;

        [ShowIf("reaction", CollisionMovementReaction.Stop)]
        public bool alsoStopRotation = true;
        
        [ShowIf("reaction", CollisionMovementReaction.Bounce)]
        public bool force2DBounce = true;
    }

    public enum CollisionMovementReaction
    {
        Ignore = 0,
        Stop = 1,
        Bounce =2
    }

    public struct StopMovementData : IComponentData
    {
    }

    public struct StopRotationData : IComponentData
    {
    }
    
    public struct BounceData : IComponentData
    {
        public bool Force2DBounce;
    }
}