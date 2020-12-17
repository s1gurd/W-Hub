using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cherry.Core.Components.Interfaces;
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
    [HideMonoScript]
    public class PerkProjectilesHitEffect : TimerBaseBehaviour, IActorAbilityTarget, IPerkAbilityForSpawned, IPerkAbilityBindable, ILevelable
    {
        [ReadOnly] public int perkLevel = 1;
        
        public bool applyToAllProjectiles = true;

        [HideIf("applyToAllProjectiles")] public string componentName = "";

        public CollisionSettings collisionSettings;

        public bool spawnTargetEffect;

        [ShowIf("spawnTargetEffect")]
        public GameObject targetEffectPrefab;

        public bool spawnProjectileEffect;

        [ShowIf("spawnProjectileEffect")]
        public GameObject projectileEffectPrefab;

        public List<MonoBehaviour> perkRelatedComponents = new List<MonoBehaviour>();

        [Space] [TitleGroup("Levelable properties")] [OnValueChanged("SetLevelableProperty")]
        public List<LevelableProperties> levelablePropertiesList = new List<LevelableProperties>();
        
        public DuplicateHandlingProperties duplicateHandlingProperties;

        public List<MonoBehaviour> PerkRelatedComponents
        {
            get
            {
                perkRelatedComponents.RemoveAll(c => ReferenceEquals(c, null));
                return perkRelatedComponents;
            }
            set => perkRelatedComponents = value;
        }

        public List<FieldInfo> LevelablePropertiesInfoCached
        {
            get
            {
                if (_levelablePropertiesInfoCached.Any()) return _levelablePropertiesInfoCached;
                return _levelablePropertiesInfoCached = this.GetFieldsWithAttributeInfo<LevelableValue>();
            }
        }

        public IActor Actor { get; set; }
        public IActor TargetActor { get; set; }
        public IActor AbilityOwnerActor { get; set; }
        
        public DuplicateHandlingProperties DuplicateHandlingProperties
        {
            get => duplicateHandlingProperties;
            set => duplicateHandlingProperties = value;
        }

        public int Level
        {
            get => perkLevel;
            set => perkLevel = value;
        }

        public List<LevelableProperties> LevelablePropertiesList
        {
            get => levelablePropertiesList;
            set => levelablePropertiesList = value;
        }
        
        public int BindingIndex { get; set; } = -1;

        private List<FieldInfo> _levelablePropertiesInfoCached = new List<FieldInfo>();

        private IActor _target;

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;

            if (!Actor.Abilities.Contains(this)) Actor.Abilities.Add(this);
            Apply(Actor);
        }

        public void Execute()
        {
            if (spawnTargetEffect)
            {
                SpawnEffect(TargetActor, targetEffectPrefab);
            }
        }

        public void Apply(IActor target)
        {
            _target = target;
            
            this.CheckPerkDuplicates(target, out var continuePerkApply);
            
            if (!continuePerkApply) return;
            
            if (target != Actor.Owner)
            {
                var ownerActorPlayer =
                    Actor.Owner.Abilities.FirstOrDefault(a => a is AbilityActorPlayer) as AbilityActorPlayer;
                
                if (ownerActorPlayer == null) return;
                
                this.SetAbilityLevel(ownerActorPlayer.Level, LevelablePropertiesInfoCached, Actor, _target);
            }

            if (!_target.AppliedPerks.Contains(this)) _target.AppliedPerks.Add(this);

            var projectiles = target.GameObject.GetComponents<AbilityWeapon>().ToList();

            if (!applyToAllProjectiles)
                projectiles = projectiles.Where(p => p.ComponentName.Equals(componentName, StringComparison.Ordinal))
                    .ToList();
            
            foreach (var p in projectiles.Where(p => p.Enabled))
            {
                p.SpawnCallbacks.Add(AddCollisionAction);

                p.SpawnCallbacks.Add(go =>
                {
                    if (!spawnProjectileEffect) return;

                    var targetActor = go.GetComponent<Actor>();

                    if (targetActor == null) return;

                    SpawnEffect(targetActor, projectileEffectPrefab);
                });
            }
        }

        public void AddCollisionAction(GameObject target)
        {
            var p = target.CopyComponent(this) as PerkProjectilesHitEffect;

            p?.AddCollision(p.gameObject);
        }

        public void AddCollision(GameObject target)
        {
            var collision = target.GetComponent<AbilityCollision>();

            if (collision != null)
            {
                collision.collisionActions.Add(new CollisionAction
                {
                    collisionLayerMask = collisionSettings.collisionLayerMask,
                    useTagFilter = collisionSettings.useTagFilter,
                    filterMode = collisionSettings.filterMode,
                    filterTags = collisionSettings.filterTags,
                    executeOnCollisionWithSpawner = collisionSettings.executeOnCollisionWithSpawner,
                    destroyAfterAction = collisionSettings.destroyAfterAction,
                    actions = new List<MonoBehaviour> {this}
                });
            }
        }

        private void SpawnEffect(IActor target, GameObject prefab)
        {
            var effectData = new ActorSpawnerSettings
            {
                objectsToSpawn = new List<GameObject> {prefab},
                SpawnPosition = SpawnPosition.UseSpawnerPosition,
                parentOfSpawns = TargetType.None,
                runSpawnActionsOnObjects = true,
                destroyAbilityAfterSpawn = true
            };

            ActorSpawn.Spawn(effectData, target, Actor);
        }

        public void Remove()
        {
            if (_target != null && _target.AppliedPerks.Contains(this)) _target.AppliedPerks.Remove(this);

            foreach (var component in perkRelatedComponents)
            {
                Destroy(component);
            }

            Destroy(this);
        }

        public void SetLevel(int level)
        {
            this.SetAbilityLevel(level, LevelablePropertiesInfoCached, Actor);
        }


        public void SetLevelableProperty()
        {
            this.SetLevelableProperty(LevelablePropertiesInfoCached);
        }
    }
}