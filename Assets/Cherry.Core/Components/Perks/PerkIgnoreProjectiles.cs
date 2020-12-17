using System;
using System.Collections;
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
using Sirenix.Utilities;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class PerkIgnoreProjectiles : CooldownBehaviour, IActorAbility, IPerkAbility, IPerkAbilityBindable,
        ILevelable, ICooldownable
    {
        [ReadOnly] public int perkLevel = 1;

        [ValueDropdown("Tags")] public string ignoredTag;
        
        public Material materialToApply;

        [InfoBox("Select materials that will not be replaced")]
        public List<Material> immutableMaterials = new List<Material>();

        public bool notRenderImmutableMaterials;

        [LevelableValue] public float perkLifetime;
        
        public float cooldownTime;
        
        public List<GameObject> FxPrefabs = new List<GameObject>();

        public List<MonoBehaviour> perkRelatedComponents = new List<MonoBehaviour>();

        [Space] [TitleGroup("Levelable properties")] [OnValueChanged("SetLevelableProperty")]
        public List<LevelableProperties> levelablePropertiesList = new List<LevelableProperties>();
        
        public DuplicateHandlingProperties duplicateHandlingProperties;

        public IActor Actor { get; set; }

        public List<MonoBehaviour> PerkRelatedComponents
        {
            get
            {
                perkRelatedComponents.RemoveAll(c => ReferenceEquals(c, null));
                return perkRelatedComponents;
            }
            set => perkRelatedComponents = value;
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

        public float CooldownTime
        {
            get => cooldownTime;
            set => cooldownTime = value;
        }

        public List<FieldInfo> LevelablePropertiesInfoCached
        {
            get
            {
                if (_levelablePropertiesInfoCached.Any()) return _levelablePropertiesInfoCached;
                return _levelablePropertiesInfoCached = this.GetFieldsWithAttributeInfo<LevelableValue>();
            }
        }

        public int BindingIndex { get; set; } = -1;
        
        public DuplicateHandlingProperties DuplicateHandlingProperties
        {
            get => duplicateHandlingProperties;
            set => duplicateHandlingProperties = value;
        }

        private List<FieldInfo> _levelablePropertiesInfoCached = new List<FieldInfo>();

        private List<Renderer> _immutableMaterialsRenderers = new List<Renderer>();
        private IActor _target;

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;

            if (!Actor.Abilities.Contains(this)) Actor.Abilities.Add(this);
        }

        public void Execute()
        {
            Apply(Actor);
        }

        public void Apply(IActor target)
        {
            _target = target;
            
            this.CheckPerkDuplicates(target, out var continuePerkApply);
            
            if (!continuePerkApply) return;

            ApplyActionWithCooldown(cooldownTime, ApplyIgnoreProjectilesPerk);
        }

        public void ApplyIgnoreProjectilesPerk()
        {
            if (_target != Actor.Owner)
            {
                var ownerActorPlayer =
                    Actor.Owner.Abilities.FirstOrDefault(a => a is AbilityActorPlayer) as AbilityActorPlayer;

                if (ownerActorPlayer == null) return;

                this.SetAbilityLevel(ownerActorPlayer.Level, LevelablePropertiesInfoCached, Actor, _target);
            }

            if (!_target.AppliedPerks.Contains(this)) _target.AppliedPerks.Add(this);

            var prevTag = _target.GameObject.tag;
            _target.GameObject.tag = ignoredTag;

            var abilityChangeMaterial = Actor.GameObject.SafeAddComponent<AbilityActorChangeMaterial>();
            abilityChangeMaterial.Actor = _target;

            abilityChangeMaterial.materialToApply = materialToApply;
            abilityChangeMaterial.immutableMaterials = immutableMaterials;
            
            abilityChangeMaterial.Execute();

            if (notRenderImmutableMaterials)
            {
                SetupImmutableMaterialsRenders(false);
            }
            
            if (FxPrefabs != null && FxPrefabs.Count > 0)
            {
                var spawnData = new ActorSpawnerSettings
                {
                    objectsToSpawn = FxPrefabs,
                    SpawnPosition = SpawnPosition.UseSpawnerPosition,
                    parentOfSpawns = TargetType.None,
                    runSpawnActionsOnObjects = true,
                    destroyAbilityAfterSpawn = true
                };

                var fx  = ActorSpawn.Spawn(spawnData, Actor, null)?.First();
            }

            Timer.TimedActions.AddAction(() =>
            {
                _target.GameObject.tag = prevTag;
                abilityChangeMaterial.TrySetOriginalMaterials();

                if (notRenderImmutableMaterials)
                {
                    SetupImmutableMaterialsRenders(true);
                }
            }, perkLifetime);
        }

        public void SetLevel(int level)
        {
            this.SetAbilityLevel(level, LevelablePropertiesInfoCached, Actor);
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

        public override void FinishTimer()
        {
            base.FinishTimer();
            this.FinishAbilityCooldownTimer(Actor);
        }

        public override void StartTimer()
        {
            base.StartTimer();
            this.StartAbilityCooldownTimer(Actor);
        }

        private void SetupImmutableMaterialsRenders(bool areEnabled)
        {
            if (_target == null) return;
            
            if (!_immutableMaterialsRenderers.Any())
            {
                var renderers = _target.GameObject.GetComponentsInChildren<Renderer>()
                    .Where(r => immutableMaterials.Contains(r.sharedMaterial))
                    .ToList();
                
                if (!renderers.Any()) return;

                _immutableMaterialsRenderers.AddRange(renderers);
            }

            _immutableMaterialsRenderers.ForEach(r => r.enabled = areEnabled);
        }
        

        public void SetLevelableProperty()
        {
            this.SetLevelableProperty(LevelablePropertiesInfoCached);
        }
        
        private static IEnumerable Tags()
        {
            return EditorUtils.GetEditorTags();
        }
    }
}