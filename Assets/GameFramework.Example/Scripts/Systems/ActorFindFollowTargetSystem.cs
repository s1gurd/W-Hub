using System;
using System.Collections.Generic;
using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
using Sirenix.Utilities;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    public class ActorFindFollowTargetSystem : ComponentSystem
    {
        private EntityQuery _queryMovement, _queryRotation;

        protected override void OnCreate()
        {
            _queryMovement = GetEntityQuery(
                ComponentType.ReadOnly<ActorFollowMovementData>(),
                ComponentType.ReadWrite<ActorNoFollowTargetMovementData>());

            _queryRotation = GetEntityQuery(
                ComponentType.ReadOnly<ActorFollowRotationData>(),
                ComponentType.ReadWrite<ActorNoFollowTargetRotationData>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_queryMovement).ForEach(
                (Entity entity, AbilityFollowMovement follow, ref ActorFollowMovementData followData) =>
                {
                    if (follow.target == null)
                    {
                        var targets = GetTargetList(follow.gameObject, follow.followTarget,
                            follow.actorWithComponentName, follow.targetTag);

                        follow.target =
                            FindActorsUtils.ChooseActor(follow.gameObject.transform, targets, follow.strategy);
                        
                        if (follow.target == null) return;
                    }

                    followData.Origin = follow.target.position;
                    PostUpdateCommands.RemoveComponent<ActorNoFollowTargetMovementData>(entity);
                }
            );

            Entities.With(_queryRotation).ForEach(
                (Entity entity, AbilityFollowRotation follow, ref ActorFollowRotationData followData) =>
                {
                    if (follow.target == null)
                    {
                        var targets = GetTargetList(follow.gameObject, follow.followTarget,
                            follow.actorWithComponentName, follow.targetTag);

                        follow.target =
                            FindActorsUtils.ChooseActor(follow.gameObject.transform, targets, follow.strategy);

                        if (follow.target == null) return;
                    }

                    followData.Origin = follow.target.rotation.eulerAngles;
                    PostUpdateCommands.RemoveComponent<ActorNoFollowTargetRotationData>(entity);
                    var t = typeof(ActorNoFollowTargetRotationData);
                    PostUpdateCommands.RemoveComponent(entity,t);
                }
            );
        }
        
        private List<Transform> GetTargetList(GameObject source, TargetType followTarget, string name, string tag)
        {
            var targets = new List<Transform>();
            
            switch (followTarget)
            {
                case TargetType.ComponentName:
                    Entities.WithAll<ActorData>().ForEach(
                        (Entity entity, Transform obj) =>
                        {
                            foreach (var component in obj.gameObject.GetComponents<IComponentName>())
                            {
                                if (component.ComponentName.Equals(name, StringComparison.Ordinal))
                                {
                                    targets.Add(obj);
                                }
                            }
                        });
                    break;
                case TargetType.ChooseByTag:
                    GameObject.FindGameObjectsWithTag(tag).ForEach(o => targets.Add(o.transform));
                    break;
                case TargetType.Spawner:
                    var t = source.GetComponent<IActor>()?.Spawner;
                    if (t != null)
                    {
                        targets.Add(t.transform);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return targets;
        }
        
        
    }
}