using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class ActorTurningFollowMovementSystem : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<ActorMovementData>(),
                ComponentType.ReadOnly<ActorRotationFollowMovementData>(),
                ComponentType.ReadOnly<Rigidbody>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_query).ForEach((Entity entity, Rigidbody rigidBody, ref ActorMovementData movement,
                ref ActorRotationFollowMovementData rotation) =>
            {
                var dir = new Vector3(movement.MovementCache.x, 0, movement.MovementCache.y);
                if (dir == Vector3.zero) return;
                var rot = rigidBody.rotation;
                var newRot = Quaternion.LookRotation(Vector3.Normalize(dir));
                if (newRot == rot) return;
                rigidBody.MoveRotation(Quaternion.Lerp(rot, newRot, Time.DeltaTime * rotation.RotationSpeed));
            });
        }
    }
}