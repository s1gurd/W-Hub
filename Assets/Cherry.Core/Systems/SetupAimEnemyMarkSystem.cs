using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Components;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    public class SetupAimEnemyMarkSystem : ComponentSystem
    {
        private EntityQuery _actorToUiQuery, _findMarkQuery, _markQuery;

        protected override void OnCreate()
        {
            _actorToUiQuery = GetEntityQuery(
                ComponentType.ReadOnly<UserInputData>(),
                ComponentType.ReadOnly<AbilityActorPlayer>());

            _findMarkQuery = GetEntityQuery(
                ComponentType.ReadOnly<Actor>(),
                ComponentType.ReadOnly<AbilityFollowMovement>(),
                ComponentType.Exclude<TargetMarkData>());

            _markQuery = GetEntityQuery(
                ComponentType.ReadOnly<Actor>(),
                ComponentType.ReadOnly<TargetMarkData>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_actorToUiQuery).ForEach(
                (Entity actorToUiEntity, AbilityActorPlayer actorPlayer) =>
                {
                    Entities.With(_findMarkQuery).ForEach(
                        (Entity markEntity, Actor actor) =>
                        {
                            if (actorPlayer.targetMarkActorComponentName != actor.ComponentName) return;
                            PostUpdateCommands.AddComponent<TargetMarkData>(markEntity);
                        }
                    );
                }
            );

            Entities.With(_markQuery).ForEach(
                (Entity markEntity, Actor markActor) =>
                {
                    if (ReferenceEquals(markActor.Spawner, null)) return;

                    var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                    var spawnerAlive = !dstManager.HasComponent<DeadActorData>(markActor.Spawner.ActorEntity) &&
                                       !dstManager.HasComponent<DestructionPendingData>(markActor.Spawner.ActorEntity);

                    var spawnerPlayerActor =
                        markActor.Spawner.Abilities.FirstOrDefault(a => a is AbilityActorPlayer) as
                            AbilityActorPlayer;

                    if (markActor.GameObject.activeSelf != spawnerAlive)
                    {
                        markActor.GameObject.SetActive(spawnerAlive);
                    }

                    if (!spawnerAlive || spawnerPlayerActor == null) return;

                    var maxDistanceThreshold = ((AbilityWeapon) spawnerPlayerActor.MaxDistanceWeapon)
                        .findTargetProperties.maxDistanceThreshold;

                    var targetPlayerTransform =
                        GetNearestEnemy(spawnerPlayerActor.Actor.GameObject.transform.position,
                            maxDistanceThreshold);

                    var markActive = targetPlayerTransform != null;

                    if (markActor.GameObject.activeSelf != markActive)
                    {
                        markActor.GameObject.SetActive(markActive);
                    }

                    if (targetPlayerTransform == null) return;

                    markActor.GameObject.transform.position = targetPlayerTransform.position;

                    var followMovement =
                        markActor.Abilities.FirstOrDefault(a =>
                            a is AbilityFollowMovement) as AbilityFollowMovement;

                    if (followMovement != null)
                    {
                        followMovement.Target = targetPlayerTransform;
                    }
                }
            );
        }

        [CanBeNull]
        private Transform GetNearestEnemy(Vector3 playerPosition, float maxDistanceThreshold)
        {
            var enemyDict = new Dictionary<Transform, float>();

            Entities.WithAll<ActorData, PlayerInputData>()
                .WithNone<DeadActorData, DestructionPendingData, UserInputData>().ForEach(
                    (Entity entity, Transform enemyTransform) =>
                    {
                        var distancesq = math.distancesq(playerPosition, enemyTransform.position);

                        if (distancesq < maxDistanceThreshold * maxDistanceThreshold)
                        {
                            enemyDict.Add(enemyTransform, distancesq);
                        }
                    }
                );

            return !enemyDict.Any() ? null : enemyDict.OrderBy(e => e.Value).FirstOrDefault().Key;
        }
    }


    public struct TargetMarkData : IComponentData
    {
    }
}