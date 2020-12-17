using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class ActorTurningFollowMovementSystemTransform : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<ActorMovementData>(),
                ComponentType.ReadOnly<ActorRotationFollowMovementData>(),
                ComponentType.Exclude<Rigidbody>(),
                ComponentType.Exclude<StopRotationData>());
        }

        protected override void OnUpdate()
        {
            var dt = Time.fixedDeltaTime;
            
            Entities.With(_query).ForEach((Entity entity, Transform transform, ref ActorMovementData movement,
                ref ActorRotationFollowMovementData rotation) =>
            {
                if (transform == null) return;
                
                var dir = new Vector3(movement.MovementCache.x, 0, movement.MovementCache.z);
                if (dir == Vector3.zero) return;
                
                var rot = transform.rotation;
                var newRot = Quaternion.LookRotation(Vector3.Normalize(dir));
                if (newRot == rot) return;
                transform.rotation = Quaternion.Lerp(rot, newRot, dt * rotation.RotationSpeed);
            });
        }
    }
}