using System;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
using Unity.Entities;
using GameFramework.Example.Utils.LowLevel;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class ActorCollisionSystem : ComponentSystem
    {
        private EntityQuery _query;

        private Collider[] _results;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<ActorColliderData>(),
                ComponentType.ReadOnly<Transform>());
            _results = new Collider[Constants.COLLISION_BUFFER_CAPACITY];
        }

        protected override void OnUpdate()
        {
            Entities.With(_query).ForEach(
                (Entity entity, AbilityCollision abilityCollision, ref ActorColliderData colliderData) =>
                {
                    var gameObject = abilityCollision.gameObject;
                    float3 position = gameObject.transform.position;
                    Quaternion rotation = gameObject.transform.rotation;
                    bool destroyAfterActions = false;
                    
                    int size = 0;

                    switch (colliderData.ColliderType)
                    {
                        case ColliderType.Sphere:
                            size = Physics.OverlapSphereNonAlloc(colliderData.SphereCenter + position,
                                colliderData.SphereRadius, _results);
                            break;
                        case ColliderType.Capsule:
                            size = Physics.OverlapCapsuleNonAlloc(colliderData.CapsuleStart + position,
                                colliderData.CapsuleEnd + position,
                                colliderData.CapsuleRadius, _results);
                            break;
                        case ColliderType.Box:
                            size = Physics.OverlapBoxNonAlloc(colliderData.BoxCenter + position,
                                colliderData.BoxHalfExtents, _results, colliderData.BoxOrientation * rotation);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (size == 0)
                    {
                        abilityCollision.ExistentCollisions.Clear();
                        return;
                    }
                    
                    int selfHit = 0;

                    if (abilityCollision.SpawnerColliders == null)
                    {
                        var spawner = abilityCollision.gameObject.GetComponent<IActor>()?.Spawner;
                        if (spawner == null) return;
                        if ((abilityCollision.SpawnerColliders = spawner.GetAllColliders()) == null) return;
                    }

                    if (abilityCollision.OwnColliders == null &&
                        (abilityCollision.OwnColliders = abilityCollision.gameObject.GetAllColliders()) == null)
                        return;

                    for (var i = 0; i < size; i++)
                    {
                        var hit = _results[i];
                        if (abilityCollision.OwnColliders.Count > 0 &&
                            abilityCollision.OwnColliders.FirstOrDefault(c => c == hit)) continue;
                        if (colliderData.initialTakeOff)
                        {
                            if (abilityCollision.SpawnerColliders.Count > 0 &&
                                abilityCollision.SpawnerColliders.FirstOrDefault(c => c == hit))
                            {
                                selfHit++;
                                continue;
                            }
                        }

                        if (abilityCollision.ExistentCollisions.Exists(c => c == hit)) continue;
                        
                        abilityCollision.ExistentCollisions.Add(hit);

                        foreach (var action in abilityCollision.collisionActions)
                        {
                            if (!action.collisionLayerMask.Contains(hit.gameObject.layer)) continue;
                            if (action.useTagFilter)
                            {
                                switch (action.filterMode)
                                {
                                    case TagFilterMode.IncludeOnly:
                                        if (!action.filterTags.Contains(hit.gameObject.tag)) continue;
                                        break;
                                    case TagFilterMode.Exclude:
                                        if (action.filterTags.Contains(hit.gameObject.tag)) continue;
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }

                            if (action.executeOnCollisionWithSpawner && abilityCollision.SpawnerColliders.Contains(hit))
                                continue;
                            
                            abilityCollision.SpawnerColliders.Add(hit);
                            
                            foreach (var a in action.actions)
                            {
                                if (a is IActorDataExchange exchange)
                                {
                                    exchange.TargetActor = hit.gameObject.GetComponent<IActor>();
                                    exchange.Execute();
                                } else if (a is IActorAbility ability) ability.Execute();
                            }
                            if (action.destroyAfterAction) destroyAfterActions = true;
                        }
                    }

                    if (selfHit == 0) colliderData.initialTakeOff = false;

                    if (destroyAfterActions)
                    {
                        PostUpdateCommands.AddComponent<ImmediateActorDestructionData>(entity);
                    }
                    for (var i = abilityCollision.ExistentCollisions.Count - 1; i > 0; i--)
                    {
                        var c = abilityCollision.ExistentCollisions[i];
                        if (!_results.FirstOrDefault(r => r == c))
                        {
                            abilityCollision.ExistentCollisions.RemoveAt(i);
                        }
                    }
                });
        }
    }
}