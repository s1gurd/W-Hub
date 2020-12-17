using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components;
using GameFramework.Example.Components.Interfaces;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    public class SuddenDeathSystem : ComponentSystem
    {
        private EntityQuery _suddenDeathZoneQuery;
        
        private Dictionary<Actor, IActorAbility> _outZonePlayersWithDamagePerks = new Dictionary<Actor, IActorAbility>();

        protected override void OnCreate()
        {
            _suddenDeathZoneQuery = GetEntityQuery(
                ComponentType.ReadOnly<Actor>(),
                ComponentType.ReadWrite<AbilitySuddenDeathZone>(),
                ComponentType.ReadOnly<ApplySuddenDeathData>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_suddenDeathZoneQuery).ForEach(
                (Entity zoneEntity, AbilitySuddenDeathZone abilitySuddenDeath) =>
                {
                    var outsideZonePlayers = OutsideZonePlayers(abilitySuddenDeath.ZoneCollider.radius,
                        abilitySuddenDeath.deathZoneTransform.position);

                    if (!outsideZonePlayers.Any() && _outZonePlayersWithDamagePerks.Count == 0) return;
                    
                    var newPlayersInZone = _outZonePlayersWithDamagePerks.Keys.Except(outsideZonePlayers).ToList();

                    foreach (var player in newPlayersInZone)
                    {
                        if (!_outZonePlayersWithDamagePerks.ContainsKey(player) ||
                            _outZonePlayersWithDamagePerks[player] == null ||
                            !(_outZonePlayersWithDamagePerks[player] is IPerkAbility)) return;
                        
                        (_outZonePlayersWithDamagePerks[player] as IPerkAbility)?.Remove();

                        _outZonePlayersWithDamagePerks.Remove(player);
                    }

                    var newPlayersOutsideZone = outsideZonePlayers.Except(_outZonePlayersWithDamagePerks.Keys);

                    foreach (var player in newPlayersOutsideZone)
                    {
                        var periodicDamagePerk = player.GameObject.AddComponent<PerkPeriodicDamage>();

                        periodicDamagePerk.TargetActor = player;
                        periodicDamagePerk.AbilityOwnerActor = player;

                        periodicDamagePerk.limitedLifespan = false;

                        periodicDamagePerk.healthDecrement = abilitySuddenDeath.healthDecrement;
                        periodicDamagePerk.applyPeriod = abilitySuddenDeath.applyPeriod;

                        var playerEntity = player.ActorEntity;
                        periodicDamagePerk.AddComponentData(ref playerEntity, player);
                        periodicDamagePerk.Execute();
                        
                        _outZonePlayersWithDamagePerks.Add(player, periodicDamagePerk);
                    }
                }
            );
        }

        private List<Actor> OutsideZonePlayers(float zoneRadius, Vector3 centerOfZone)
        {
            var playerList = new List<Actor>();

            Entities.WithAll<ActorData, PlayerInputData, Actor>()
                .WithNone<DeadActorData, DestructionPendingData>().ForEach(
                    (Entity entity, Actor actorPlayer, Transform playerTransform) =>
                    {
                        var distancesq = math.distancesq(playerTransform.position, centerOfZone);

                        if (distancesq > zoneRadius * zoneRadius)
                        {
                            playerList.Add(actorPlayer);
                        }
                    }
                );

            return playerList;
        }
    }
}