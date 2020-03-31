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
    [HideMonoScript]
    [DoNotAddToEntity]
    public class AbilitySpawnProjectile : MonoBehaviour, IActorAbility, IActorSpawner, ITimer, IComponentName
    {
        public string ComponentName => _componentName;

        [Space] [ShowInInspector] [SerializeField]
        private string _componentName = null;

        [Space] public ActorSpawnerSettings SpawnData;

        //TODO: Consider making this class child of AbilityActorSpawn, and leave all common fields to parent

        [Space] public float projectileStartupDelay = 0f;
        public float projectileReloadTime = 0.3f;

        [InfoBox("Clip Capacity of 0 stands for unlimited clip")]
        public int projectileClipCapacity = 0;

        [HideIf("projectileClipCapacity", 0f)] public float clipReloadTime = 1f;

        [InfoBox("Put here IEnable implementation to display reload")] [Space]
        public List<MonoBehaviour> reloadDisplayToggle = new List<MonoBehaviour>();

        [HideIf("projectileClipCapacity", 0f)] [Space]
        public List<MonoBehaviour> clipReloadDisplayToggle = new List<MonoBehaviour>();

        public ActorProjectileSpawnAnimProperties actorProjectileSpawnAnimProperties;

        public List<GameObject> SpawnedObjects { get; private set; }

        public bool TimerActive
        {
            get => isEnabled;
            set => isEnabled = value;
        }

        public TimerComponent Timer => _timer = this.gameObject.GetOrCreateTimer(_timer);

        private Entity _entity;

        [HideInInspector] public bool isEnabled = true;

        private int _projectileClip;
        private TimerComponent _timer;

        public void AddComponentData(ref Entity entity)
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            _entity = entity;

            _projectileClip = projectileClipCapacity;

            //This is not necessary at all, just to cache timer component 
            _timer = this.gameObject.GetOrCreateTimer(_timer);

            dstManager.AddComponent<TimerData>(entity);

            if (actorProjectileSpawnAnimProperties.HasActorProjectileAnimation)
            {
                dstManager.AddComponentData(entity, new ActorProjectileAnimData
                {
                    AnimHash = Animator.StringToHash(actorProjectileSpawnAnimProperties.ActorProjectileAnimationName)
                });
            }
        }


        public void Execute()
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator Here we need exact comparison
            if (isEnabled && projectileStartupDelay == 0)
            {
                Spawn();

                World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentData(_entity,
                    new ActorProjectileThrowAnimData());

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (projectileReloadTime == 0) return;

                StartTimer();
                Timer.TimedActions.AddAction(FinishTimer, projectileReloadTime);

                if (projectileClipCapacity == 0) return;

                _projectileClip--;
                if (_projectileClip < 1)
                {
                    Timer.TimedActions.AddAction(Reload, clipReloadTime);
                }
            }
            else if (isEnabled)
            {
                Timer.TimedActions.AddAction(Spawn, projectileStartupDelay);
            }
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

        public void Reload()
        {
            _projectileClip = projectileClipCapacity;
        }

        public void FinishTimer()
        {
            isEnabled = true;
        }

        public void StartTimer()
        {
            isEnabled = false;
        }
    }

    public struct ActorProjectileAnimData : IComponentData
    {
        public int AnimHash;
    }

    public struct ActorProjectileThrowAnimData : IComponentData
    {
    }
}