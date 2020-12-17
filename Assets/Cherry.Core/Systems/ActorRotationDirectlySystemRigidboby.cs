using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class ActorRotationDirectlySystemRigidboby : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<RotateDirectlyData>(),
                ComponentType.Exclude<ActorNoFollowTargetRotationData>(),
                ComponentType.ReadOnly<Rigidbody>(),
                ComponentType.Exclude<StopRotationData>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_query).ForEach(
                (Entity entity, Rigidbody transform, ref RotateDirectlyData rotation) =>
                {
                    if (transform == null) return;
                    float3 currentRotation = transform.rotation.eulerAngles;
                    float3 newRotation = currentRotation;

                    for (var i = 0; i < 3; i++)
                    {
                        if (rotation.Constraints[i])
                        {
                            newRotation[i] = rotation.Rotation[i];
                        }
                    }

                    transform.rotation = Quaternion.Euler(newRotation);
                });
        }
    }
}