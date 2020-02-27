using System.Collections.Generic;
using GameFramework.Example.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Loading.LevelBootstrap
{
    [HideMonoScript]
    public sealed class LevelActorSpawnBehaviour : MonoBehaviour, IActorSpawner, IComponentName
    {
        public string ComponentName => _componentName;

        [Space] [ShowInInspector] [SerializeField] private string _componentName = null;
        [Space] 
        
        public ActorSpawnerSettings SpawnData;

        public List<GameObject> SpawnedObjects { get; private set; }
        
        public void Spawn()
        {
            SpawnedObjects = ActorSpawn.Spawn(SpawnData);
        }

        public void RunSpawnActions()
        {
            if (SpawnData.RunSpawnActionsOnObjects)
            {
                _ = ActorSpawn.RunSpawnActions(SpawnedObjects);
            }
        }
    }
}