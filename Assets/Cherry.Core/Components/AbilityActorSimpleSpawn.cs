using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Loading;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [DoNotAddToEntity]
    [HideMonoScript]
    public class AbilityActorSimpleSpawn : MonoBehaviour, IActorAbility, IComponentName
    {
        [Space] [SerializeField]
        public string componentName = "";
        
        public GameObject objectToSpawn;
        
        [EnumToggleButtons] public SpawnerType spawnerType;

        [EnumToggleButtons] public OwnerType ownerType;
        
        [Space] public bool ExecuteOnAwake = false;
        [Space] public bool DestroyAfterSpawn = false;

        [HideInInspector] public GameObject spawnedObject;
        
        public IActor Actor { get; set; }
        
        public string ComponentName
        {
            get => componentName;
            set => componentName = value;
        }

        private IActor _currentSpawner;
        private IActor _currentOwner;
        
        void Start()
        {
            if (ExecuteOnAwake) Execute();
        }

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
        }

        public void Execute()
        {
            _currentSpawner = spawnerType == SpawnerType.CurrentActor
                ? Actor
                : null;

            _currentOwner = ownerType == OwnerType.CurrentActorOwner
                ? Actor.Owner
                : ownerType == OwnerType.CurrentActor
                    ? Actor
                    : null;

            var spawnData = new ActorSpawnerSettings
            {
                objectsToSpawn = new List<GameObject> {objectToSpawn},
                SpawnPosition = SpawnPosition.UseSpawnerPosition,
                parentOfSpawns = TargetType.None,
                runSpawnActionsOnObjects = true,
                destroyAbilityAfterSpawn = true
            };

            spawnedObject = ActorSpawn.Spawn(spawnData, _currentSpawner, _currentOwner)?.First();
            
            if (DestroyAfterSpawn) Destroy(this);
        }
    }

    public enum SpawnerType
    {
        CurrentActor = 0,
        Null = 1
    }

    public enum OwnerType
    {
        CurrentActorOwner = 0,
        CurrentActor = 1,
        Null = 2
    }
}