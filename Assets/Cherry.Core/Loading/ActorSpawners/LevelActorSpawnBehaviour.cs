using System.Collections.Generic;

using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;

using UnityEngine;

namespace GameFramework.Example.Loading.ActorSpawners
{
    [HideMonoScript][DoNotAddToEntity]
    public sealed class LevelActorSpawnBehaviour : MonoBehaviour, IActorSpawner, IComponentName
    {
        public string ComponentName
        {
            get => componentName;
            set => componentName = value;
        }

        [Space] [SerializeField] public string componentName = "";
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