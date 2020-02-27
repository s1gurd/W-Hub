using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class ActorMovementByInputSystemRigidbody : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<MoveByInputData>(),
                ComponentType.ReadOnly<ActorMovementData>(),
                ComponentType.ReadOnly<Rigidbody>());
        }

        protected override void OnUpdate()
        {
            var dt = Time.DeltaTime;
            var t = Time.time;

            Entities.With(_query).ForEach(
                (Entity entity, Rigidbody rigidBody, ref ActorMovementData movement) =>
                {
                    var speed = movement.MovementSpeed;

                    var multiplier = MathUtils.ApplyDynamics(ref movement, t);

                    var movementDelta = speed * dt * multiplier * movement.ExternalMultiplier *
                                        Vector3.ClampMagnitude(
                                            new Vector3(movement.MovementCache.x, 0, movement.MovementCache.y), 1f);

                    if (movementDelta == Vector3.zero) return;
                    
                    var go = rigidBody.gameObject;
                    var position = go.transform.position;
                    var newPos = new Vector3(position.x, position.y, position.z) + movementDelta;
                    rigidBody.MovePosition(newPos);
                }
            );
        }
    }
}