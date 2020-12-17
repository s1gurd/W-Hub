using System.Collections.Generic;

using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Assertions;

namespace GameFramework.Example.Loading.ActorSpawners
{
    [HideMonoScript][DoNotAddToEntity]
    public class LevelActorSpawnFromSO : MonoBehaviour, IActorSpawner, IComponentName
    {
        public string ComponentName
        {
            get => componentName;
            set => componentName = value;
        }

        [Space] [SerializeField] public string componentName = "";
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