using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components;
using GameFramework.Example.Enums;
using GameFramework.Example.Loading;
using JetBrains.Annotations;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Utils
{
    public static class ActorUtils
    {
        public static void ChangeActorForceMovementData(this IActor target, Vector3 forwardVector)
        {
            if (target == null) return;

            if (!World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<ActorForceMovementData>(target.ActorEntity)) return;

            var actorForceMovementData = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<ActorForceMovementData>(target.ActorEntity);

            actorForceMovementData.MoveDirection = MoveDirection.UseDirection;
            actorForceMovementData.ForwardVector = forwardVector;
            actorForceMovementData.CompensateSpawnerRotation = false;

            World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(target.ActorEntity, actorForceMovementData);
        }
        
        [CanBeNull]
        public static List<GameObject> SimpleSpawnObjects(this IActor spawner, List<GameObject> objectToSpawn)
        {
            if (objectToSpawn == null || !objectToSpawn.Any()) return null;
            
            var spawnData = new ActorSpawnerSettings
            {
                objectsToSpawn = objectToSpawn,
                SpawnPosition = SpawnPosition.UseSpawnerPosition,
                parentOfSpawns = TargetType.None,
                runSpawnActionsOnObjects = true,
                destroyAbilityAfterSpawn = true
            };

            return ActorSpawn.Spawn(spawnData, spawner, null);
        }
    }
}