using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class ActorFollowMovementSystem : ComponentSystem
    {
        private EntityQuery _queryByInput, _queryDirectly;

        protected override void OnCreate()
        {
            _queryByInput = GetEntityQuery(
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<ActorFollowMovementData>(),
                ComponentType.ReadOnly<ActorMovementData>(),
                ComponentType.ReadOnly<MoveByInputData>(),
                ComponentType.Exclude<ActorNoFollowTargetMovementData>());

            _queryDirectly = GetEntityQuery(
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<ActorFollowMovementData>(),
                ComponentType.ReadOnly<MoveDirectlyData>(),
                ComponentType.Exclude<ActorNoFollowTargetMovementData>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_queryByInput).ForEach(
                (Entity entity, AbilityFollowMovement follow, ref ActorMovementData movement) =>
                {
                    if (follow.target == null)
                    {
                        PostUpdateCommands.AddComponent<ActorNoFollowTargetMovementData>(entity);
                        return;
                    }

                    float3 delta = follow.target.position - follow.gameObject.transform.position;
                    if (math.abs(delta.x) < Constants.FOLLOW_MOVEMENT_AXIS_THRESH) delta.x = 0f;
                    if (math.abs(delta.z) < Constants.FOLLOW_MOVEMENT_AXIS_THRESH) delta.z = 0f;
                    movement.Input = new float2(delta.x, delta.z);
                }
            );

            Entities.With(_queryDirectly).ForEach(
                (Entity entity, AbilityFollowMovement follow, ref MoveDirectlyData movement,
                    ref ActorFollowMovementData data) =>
                {
                    if (follow.target == null)
                    {
                        PostUpdateCommands.AddComponent<ActorNoFollowTargetMovementData>(entity);
                        return;
                    }
                    
                    float3 targetPosition = follow.target.position;
                    movement.Position = targetPosition;
                    if (follow.retainOffset) movement.Position -= data.Origin;
                }
            );
        }
    }
}