using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    [DoNotAddToEntity]
    public class AbilityActorSpawn2 : MonoBehaviour, IActorAbility, IActorSpawner, IComponentName,
        IDeclareReferencedPrefabs
    {
        public string ComponentName => _componentName;

        [Space] [ShowInInspector] [SerializeField]
        private string _componentName = null;

        [Space] public ActorSpawnerSettings SpawnData;

        public List<GameObject> SpawnedObjects { get; private set; }

        private Entity _entity;


        public void AddComponentData(ref Entity entity)
        {
            
            
        }

        public void Execute()
        {
            Spawn();
        }

        public void Spawn()
        {
            /*var entities = World.Active.EntityManager.GetAllEntities().ToList();
            _entity = entities.FirstOrDefault(e =>
                World.Active.EntityManager.GetName(e).Contains(SpawnData.objectsToSpawn[0].name));

            var c = World.Active.GetExistingSystem<EntityCommandBufferSystem>().CreateCommandBuffer();
            c.ToConcurrent();
            c.Instantiate(_entity);*/
            
            //World.Active.EntityManager.Instantiate(_entity);

            SpawnedObjects = ActorSpawn.Spawn(SpawnData, this.gameObject);
        }

        public void RunSpawnActions()
        {
            if (SpawnData.RunSpawnActionsOnObjects)
            {
                _ = ActorSpawn.RunSpawnActions(SpawnedObjects);
            }
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.AddRange(SpawnData.objectsToSpawn);
        }
    }
}