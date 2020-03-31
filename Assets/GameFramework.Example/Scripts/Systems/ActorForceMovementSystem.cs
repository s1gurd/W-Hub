using System;
using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using Unity.Mathematics;

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
                            if (forceMovementData.stopGuiding) break;
                            if (!CheckSpawner(forceMovement)) return;
                            forceMovementData.ForwardVector = forceMovement.spawner.forward;
                            forceMovementData.stopGuiding = true;
                            break;
                        case MoveDirection.UseDirection:
                            if (!forceMovementData.stopGuiding && forceMovementData.CompensateSpawnerRotation)
                            {
                                forceMovementData.ForwardVector -= (float3)forceMovement.spawner.forward;
                                forceMovementData.stopGuiding = true;
                            } 
                            break;
                        case MoveDirection.GuidedBySpawner:
                            if (!CheckSpawner(forceMovement)) return;
                            forceMovementData.ForwardVector = forceMovement.spawner.forward;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    
                    movement.Input = forceMovementData.ForwardVector;
                }
            );
        }

        private bool CheckSpawner(AbilityForceMovement forceMovement)
        {
            if (forceMovement.spawner == null)
            {
                forceMovement.GetSpawner();
            }

            return forceMovement.spawner != null;
        }
    }
}