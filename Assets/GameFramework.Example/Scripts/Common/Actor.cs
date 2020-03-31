using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils.LowLevel;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Common
{
    [HideMonoScript]
    public class Actor : MonoBehaviour, IActor, IComponentName, IConvertGameObjectToEntity
    {
        public string ComponentName => _componentName;

        [Space] [ShowInInspector] [SerializeField]
        private string _componentName = null;

        [Space] [ValidateInput("MustBeAbility", "Ability MonoBehaviours must derive from IActorAbility!")]
        public List<MonoBehaviour> ExecuteOnSpawn = new List<MonoBehaviour>();

        public Entity ActorEntity { get; set; }
        public EntityManager WorldEntityManager { get; set; }

        public List<string> ComponentNames { get; } = new List<string>();
        public List<IActorAbility> Abilities { get; private set; } = new List<IActorAbility>();

        public GameObject Spawner
        {
            get => _spawner;
            set => _spawner = value;
        }

        private GameObject _spawner;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            ActorEntity = entity;
            WorldEntityManager = dstManager;

            if (!ComponentName.Equals(string.Empty)) ComponentNames.Add(this.ComponentName);
            dstManager.AddComponentData(entity, new ActorData());

            Abilities = GetComponents<IActorAbility>().ToList();

            foreach (var ability in Abilities)
            {
                ability.AddComponentData(ref entity);
                if (ability is IComponentName componentName && !componentName.ComponentName.Equals(string.Empty))
                {
                    ComponentNames.Add(componentName.ComponentName);
                }
            }
        }

        private void Awake()
        {
            if (World.DefaultGameObjectInjectionWorld == null)
            {
                Debug.LogError(
                    "[ACTOR CONVERT TO ENTITY] Convert Entity failed because there was no Active World");
                return;
            }

            // Root ConvertToEntity is responsible for converting the whole hierarchy
            if (transform.parent != null && transform.parent.GetComponentInParent<ConvertToEntity>() != null)
                return;

            EntityConversionUtils.ConvertAndInjectOriginal(this.gameObject);
        }


        public void PerformSpawnActions()
        {
            foreach (var component in ExecuteOnSpawn)
            {
                if (!(component is IActorAbility))
                {
                    Debug.LogError($"[ACTOR ABILITY EXECUTION] \"{component.name}\" is not an ability!");
                    continue;
                }

                (component as IActorAbility).Execute();
            }
        }


        private bool MustBeAbility(List<MonoBehaviour> actions)
        {
            foreach (var action in actions)
            {
                if (action is IActorAbility || action is null) continue;

                return false;
            }

            return true;
        }
    }


    public struct ActorData : IComponentData
    {
    }
}