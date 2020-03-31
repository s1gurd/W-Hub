using System.Collections.Generic;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript][DoNotAddToEntity]
    public class AbilityActorSpawn : MonoBehaviour, IActorAbility, IActorSpawner, IComponentName
    {
        public string ComponentName => _componentName;

        [Space] [ShowInInspector] [SerializeField] private string _componentName = null;
        [Space] 
        
        public ActorSpawnerSettings SpawnData;

        public List<GameObject> SpawnedObjects { get; private set; }

        public void AddComponentData(ref Entity entity)
        {
        }

        public void Execute()
        {
            Spawn();
            RunSpawnActions();
        }
        
        public void Spawn()
        {
            SpawnedObjects = ActorSpawn.Spawn(SpawnData, this.gameObject);
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