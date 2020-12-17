using System;
using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class ActorForceMovementSystem : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<AbilityForceMovement>(),
                ComponentType.ReadOnly<ActorMovementData>(),
                ComponentType.ReadOnly<ActorForceMovementData>(),
                ComponentType.Exclude<StopMovementData>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_query).ForEach(
                (Entity entity, AbilityForceMovement forceMovement, ref ActorForceMovementData forceMovementData,
                    ref ActorMovementData movement) =>
                {
                    switch (forceMovementData.MoveDirection)
                    {
                        case MoveDirection.SpawnerForward:
                            if (forceMovementData.stopGuiding || forceMovement.Actor.Spawner == null) return;
                            forceMovementData.ForwardVector = forceMovement.Spawner.forward;
                            forceMovementData.stopGuiding = true;
                            break;
                        case MoveDirection.UseDirection:
                            if (!forceMovementData.stopGuiding && forceMovementData.CompensateSpawnerRotation)
                            {
                                forceMovementData.ForwardVector -= (float3)forceMovement.Spawner.forward;
                                forceMovementData.stopGuiding = true;
                            } 
                            break;
                        case MoveDirection.GuidedBySpawner:
                            if (forceMovement.Actor.Spawner == null) return;
                            forceMovementData.ForwardVector = forceMovement.Spawner.forward;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    movement.Input = forceMovementData.ForwardVector;
                }
            );
        }
    }
}