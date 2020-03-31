using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class ActorFollowRotationSystem : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<ActorFollowRotationData>(),
                ComponentType.ReadOnly<RotateDirectlyData>(),
                ComponentType.Exclude<ActorNoFollowTargetRotationData>(),
                ComponentType.Exclude<StopRotationData>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_query).ForEach(
                (Entity entity, AbilityFollowRotation follow, ref RotateDirectlyData rotation,
                    ref ActorFollowRotationData data) =>
                {
                    if (follow.target == null)
                    {
                        PostUpdateCommands.AddComponent<ActorNoFollowTargetRotationData>(entity);
                        return;
                    }

                    float3 targetEuler = follow.target.rotation.eulerAngles;
                    
                    var newRotation = follow.retainRotationOffset ? targetEuler - data.Origin : targetEuler;
                    rotation.Rotation = newRotation;
                }
            );
        }
    }
}