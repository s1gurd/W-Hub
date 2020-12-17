using System;
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
    public class PerkModifyWeaponSize : MonoBehaviour, IActorAbilityTarget, IPerkAbilityForSpawned
    {
        public bool ApplyToAllProjectiles = true;

        [HideIf("ApplyToAllProjectiles")] public string componentName = "";

        public float sizeModifier;
        public CollisionSettings collisionSettings;
        
        public  bool spawnTargetEffect;
        
        [ShowIf("spawnTargetEffect")]
        public GameObject targetEffectPrefab;
        
        public DuplicateHandlingProperties duplicateHandlingProperties;

        public IActor Actor { get; set; }
        public IActor TargetActor { get; set; }
        public IActor AbilityOwnerActor { get; set; }
        
        public DuplicateHandlingProperties DuplicateHandlingProperties
        {
            get => duplicateHandlingProperties;
            set => duplicateHandlingProperties = value;
        }

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
            
            if (!Actor.AppliedPerks.Contains(this)) Actor.AppliedPerks.Add(this);
        }

        public void Execute()
        {
            if (spawnTargetEffect)
            {
                SpawnEffect(TargetActor);
            }
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
                    actions = new List<MonoBehaviour>{this}
                });
            }
        }

        public void Apply(IActor target)
        {
            this.CheckPerkDuplicates(target, out var continuePerkApply);
            
            if (!continuePerkApply) return;
            
            var copy = target.GameObject.CopyComponent(this) as PerkModifyWeaponSize;
            
            if (copy == null)
            {
                Debug.LogError("[PERK MODIFY WEAPON SIZE] Error copying perk to Actor!");
                return;
            }
            
            if (!Actor.Spawner.AppliedPerks.Contains(copy)) Actor.Spawner.AppliedPerks.Add(copy);
            
            var projectiles = target.GameObject.GetComponents<AbilityWeapon>().ToList();
            
            if (!ApplyToAllProjectiles)
                projectiles = projectiles
                    .Where(p => p.ComponentName.Equals(componentName, StringComparison.Ordinal))
                    .ToList();
            
            foreach (var p in projectiles)
            {
                p.SpawnCallbacks.Add(copy.ApplySizeModifier);
                p.SpawnCallbacks.Add(copy.AddCollisionAction);
            }
        }

        public void AddCollisionAction(GameObject target)
        {
            var p = target.CopyComponent(this) as PerkModifyWeaponSize;
            
            p?.AddCollision(p.gameObject);
        }

        private void ApplySizeModifier(GameObject target)
        {
            target.transform.localScale *= sizeModifier;
        }

        private void SpawnEffect(IActor target)
        {
            var effectData = new ActorSpawnerSettings
            {
                objectsToSpawn = new List<GameObject> {targetEffectPrefab},
                SpawnPosition = SpawnPosition.UseSpawnerPosition,
                parentOfSpawns = TargetType.None,
                runSpawnActionsOnObjects = true,
                destroyAbilityAfterSpawn = true
            };
            
            ActorSpawn.Spawn(effectData, target, Actor);
        }
        
        public void Remove()
        {
            if (Actor.Spawner.AppliedPerks.Contains(this)) Actor.Spawner.AppliedPerks.Remove(this);
            Destroy(this);
        }
    }
}