using System.Collections.Generic;
using GameFramework.Example.Common;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameFramework.Example.Loading.LevelBootstrap
{
    [HideMonoScript]
    public class LevelActorSpawnFromSO : MonoBehaviour, IActorSpawner, IComponentName
    {
        public string ComponentName => _componentName;

        [Space] [ShowInInspector] [SerializeField] private string _componentName = null;
        [Space]
        
        public LevelActorSpawnerDataSO spawnDataFile;

        public List<GameObject> SpawnedObjects { get; private set; }
        
        public void Spawn()
        {
            Assert.IsNotNull(spawnDataFile);
            SpawnedObjects = ActorSpawn.Spawn(spawnDataFile.SpawnData);
        }

        public void RunSpawnActions()
        {
            _ = ActorSpawn.RunSpawnActions(SpawnedObjects);
        }
    }
}