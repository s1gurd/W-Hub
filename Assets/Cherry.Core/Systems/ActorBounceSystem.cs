using System;
using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Object = System.Object;


namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    [UpdateAfter(typeof(ActorCollisionSystem))]
    public class ActorBounceSystem : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<ActorMovementData>(),
                ComponentType.ReadOnly<ActorColliderData>(),
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadWrite<BounceData>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_query).ForEach(
                (Entity entity, Transform transform, ref ActorMovementData movement,
                    ref ActorColliderData colliderData, ref BounceData bounce) =>
                {
                    World.EntityManager.RemoveComponent<BounceData>(entity);

                    float3 currentCenterPosition;
                    float3 prevCenterPosition;
                    float distance;

                    RaycastHit hit;

                    switch (colliderData.ColliderType)
                    {
                        case ColliderType.Sphere:
                            currentCenterPosition = colliderData.SphereCenter + (float3) transform.position;
                            prevCenterPosition = currentCenterPosition - movement.MovementCache;

                            distance = math.distancesq(prevCenterPosition, currentCenterPosition);

                            if (!Physics.SphereCast(prevCenterPosition, colliderData.SphereRadius, movement.Input,
                                out hit, distance)) return;

                            break;
                        case ColliderType.Capsule:
                            var capsuleCenter = (colliderData.CapsuleEnd + colliderData.CapsuleStart) * 0.5f;
                            var goPosition = transform.position;

                            currentCenterPosition = capsuleCenter + (float3) goPosition;
                            prevCenterPosition = currentCenterPosition - movement.MovementCache;

                            distance = math.distancesq(prevCenterPosition, currentCenterPosition) * 2;

                            var prevCapsuleStart =
                                colliderData.CapsuleStart + (float3) goPosition - movement.MovementCache;
                            var presCapsuleEnd = colliderData.CapsuleEnd + (float3) goPosition - movement.MovementCache;

                            if (!Physics.CapsuleCast(prevCapsuleStart, presCapsuleEnd,
                                colliderData.CapsuleRadius, movement.Input, out hit, distance)) return;

                            break;
                        case ColliderType.Box:
                            currentCenterPosition = colliderData.BoxCenter + (float3) transform.position;
                            prevCenterPosition = currentCenterPosition - movement.MovementCache;

                            distance = math.distancesq(prevCenterPosition, currentCenterPosition);

                            if (!Physics.BoxCast(prevCenterPosition, colliderData.BoxHalfExtents, movement.Input,
                                out hit, colliderData.BoxOrientation, distance)) return;

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    var newInput = movement.Input.Reflect(hit.normal);
                    if (bounce.Force2DBounce) newInput.y = 0;

                    if (Math.Abs(math.distancesq(float3.zero, newInput) - 1f) > Constants.INPUT_SQDIST_THRESH)
                        newInput = math.normalize(newInput);

                    movement.Input = newInput;
                });
        }
    }
}