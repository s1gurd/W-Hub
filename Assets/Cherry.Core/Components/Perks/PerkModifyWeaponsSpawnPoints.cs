using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    public class PerkModifyWeaponsSpawnPoints : MonoBehaviour, IActorAbilityTarget, IPerkAbility
    {
        [SerializeField] private string _perkName;

        public bool ApplyToAllProjectiles = true;

        [HideIf("ApplyToAllProjectiles")] public string componentName = "";

        public List<GameObject> spawnPoints = new List<GameObject>();

        public bool suppressOriginalWeaponSpawn = false;
        
        public DuplicateHandlingProperties duplicateHandlingProperties;
        public IActor Actor { get; set; }
        public IActor TargetActor { get; set; }
        public IActor AbilityOwnerActor { get; set; }
        
        public DuplicateHandlingProperties DuplicateHandlingProperties
        {
            get => duplicateHandlingProperties;
            set => duplicateHandlingProperties = value;
        }

        private List<GameObject> _copiedToTargetSpawnPoints { get; } = new List<GameObject>();

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
        }

        public void Execute()
        {
        }

        public void Apply(IActor target)
        {
            if (target == null) return;
            
            this.CheckPerkDuplicates(target, out var continuePerkApply);
            
            if (!continuePerkApply) return;

            TargetActor = target;

            var copy = target.GameObject.CopyComponent(this) as PerkModifyWeaponsSpawnPoints;

            if (copy == null)
            {
                Debug.LogError("[PERK MODIFY WEAPONS SPAWN POINTS] Error copying perk to Actor!");
                return;
            }

            if (!Actor.Spawner.AppliedPerks.Contains(copy)) Actor.Spawner.AppliedPerks.Add(copy);

            foreach (var spawnPoint in spawnPoints)
            {
                var newSpawnPoint = Instantiate(spawnPoint.gameObject, target.GameObject.transform);
                newSpawnPoint.transform.localPosition = spawnPoint.transform.localPosition;

                _copiedToTargetSpawnPoints.Add(newSpawnPoint);
            }

            var projectiles = target.GameObject.GetComponents<AbilityWeapon>().ToList();

            if (!ApplyToAllProjectiles)
                projectiles = projectiles
                    .Where(p => p.ComponentName.Equals(componentName, StringComparison.Ordinal))
                    .ToList();

            foreach (var p in projectiles)
            {
                if (p.appliedPerksNames.Contains(_perkName)) continue;

                var spawnData = p.projectileSpawnData;
                spawnData.SpawnPosition = SpawnPosition.UseSpawnPoints; 
                spawnData.SpawnPointsFillingMode = FillOrder.SequentialOrder;
                spawnData.FillSpawnPoints = FillMode.FillAllSpawnPoints;
                
                var weaponTransform = p.gameObject.transform;
                
                var existingBaseSpawnPoint = spawnData.SpawnPoints.FirstOrDefault(point =>
                    point.transform.position == weaponTransform.position &&
                    point.transform.rotation == weaponTransform.rotation);
                

                if (!suppressOriginalWeaponSpawn && existingBaseSpawnPoint == null && !p.appliedPerksNames.Any())
                {
                    var baseSpawnPoint = new GameObject("Base Spawn Point");
                    baseSpawnPoint.transform.SetParent(p.SpawnPointsRoot);

                    baseSpawnPoint.transform.localPosition = Vector3.zero;
                    baseSpawnPoint.transform.localRotation = Quaternion.identity;
                
                    spawnData.SpawnPoints.Add(baseSpawnPoint);
                }

                if (suppressOriginalWeaponSpawn && existingBaseSpawnPoint != null)
                {
                    spawnData.SpawnPoints.Remove(existingBaseSpawnPoint);
                }

                foreach (var spawnPoint in _copiedToTargetSpawnPoints)
                {
                    spawnPoint.transform.SetParent(p.SpawnPointsRoot);
                    spawnData.SpawnPoints.Add(spawnPoint);
                }

                p.projectileSpawnData = spawnData;
                p.appliedPerksNames.Add(_perkName);

                p.SpawnCallbacks.Add(go =>
                {
                    var targetActor = go.GetComponent<Actor>();
                    if (targetActor == null) return;
                    targetActor.ChangeActorForceMovementData(go.transform.forward);
                });
            }
        }

        public void Remove()
        {
            if (Actor.Spawner.AppliedPerks.Contains(this)) Actor.Spawner.AppliedPerks.Remove(this);
        }
    }
}