using System;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityCollisionMovement : MonoBehaviour, IActorAbility
    {
        [EnumToggleButtons]
        public CollisionMovementReaction reaction;

        [ShowIf("reaction", CollisionMovementReaction.Stop)]
        public bool alsoStopRotation = true;
        
        private Entity _entity;
        public void AddComponentData(ref Entity entity)
        {
            _entity = entity;
        }

        public void Execute()
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            switch (reaction)
            {
                case CollisionMovementReaction.Ignore:
                    break;
                case CollisionMovementReaction.Stop:
                    dstManager.AddComponent<StopMovementData>(_entity);
                    if (alsoStopRotation) dstManager.AddComponent<StopRotationData>(_entity);
                    break;
                case CollisionMovementReaction.Bounce:
                    Debug.LogError("[COLLISION MOVEMENT REACTION] Bounce not yet implemented, ignoring");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
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
}