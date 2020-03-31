using System.Collections.Generic;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using UnityEngine;

namespace GameFramework.Example.Loading.LevelBootstrap
{
    public sealed class LevelActorFromMeta : IActorSpawner
    {
        public List<GameObject> SpawnedObjects { get; set; }

        public IActorSpawnerSettings ActorSpawnerSettings { get; set; }

        void IActorSpawner.Spawn()
        {
            SpawnedObjects = ActorSpawn.Spawn(ActorSpawnerSettings);
        }

        void IActorSpawner.RunSpawnActions()
        {
            _ = ActorSpawn.RunSpawnActions(SpawnedObjects);
        }
    }
}