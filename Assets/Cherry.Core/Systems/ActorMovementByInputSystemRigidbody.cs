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
                ComponentType.ReadOnly<Rigidbody>(),
                ComponentType.Exclude<StopMovementData>());
        }

        protected override void OnUpdate()
        {
            var dt = Time.fixedDeltaTime;
            var t = (float)Time.ElapsedTime;

            Entities.With(_query).ForEach(
                (Entity entity, Rigidbody rigidBody, ref ActorMovementData movement) =>
                {
                    if (rigidBody == null) return;
                    
                    var speed = movement.MovementSpeed;
                    float multiplier;

                    if (movement.Dynamics.useDynamics)
                    {
                        multiplier = MathUtils.ApplyDynamics(ref movement, t);
                    }
                    else
                    {
                        multiplier = 1f;
                        movement.MovementCache = movement.Input;
                    }
                    
                    var movementDelta = speed * dt * multiplier * movement.ExternalMultiplier *
                                        Vector3.ClampMagnitude(movement.MovementCache, 1f);
                    
                    //Debug.Log(rigidBody.gameObject.name + " / " + movementDelta);

                    if (movementDelta == Vector3.zero) return;
                    
                    var go = rigidBody.gameObject;
                    var position = go.transform.position;
                    var newPos = position + movementDelta;
                    rigidBody.MovePosition(newPos);
                    
                }
            );
        }
    }
}