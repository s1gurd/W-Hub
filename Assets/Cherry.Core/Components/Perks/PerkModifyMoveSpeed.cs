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
    public class PerkModifyMoveSpeed : CooldownBehaviour, IActorAbility, IPerkAbility, IPerkAbilityBindable,
        ILevelable, ICooldownable
    {
        [ReadOnly] public int perkLevel = 1;

        public float moveSpeedMultiplier = 1.5f;

        public bool limitedLifespan;

        [ShowIf("limitedLifespan")] [LevelableValue]
        public float lifespan;

        public float cooldownTime;

        public GameObject moveFX;

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

        public List<FieldInfo> LevelablePropertiesInfoCached
        {
            get
            {
                if (_levelablePropertiesInfoCached.Any()) return _levelablePropertiesInfoCached;
                return _levelablePropertiesInfoCached = this.GetFieldsWithAttributeInfo<LevelableValue>();
            }
        }

        public int BindingIndex { get; set; } = -1;

        public float CooldownTime
        {
            get => cooldownTime;
            set => cooldownTime = value;
        }
        
        public DuplicateHandlingProperties DuplicateHandlingProperties
        {
            get => duplicateHandlingProperties;
            set => duplicateHandlingProperties = value;
        }

        private List<FieldInfo> _levelablePropertiesInfoCached = new List<FieldInfo>();

        private IActor _target;

        private EntityManager _dstManager;

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;

            _dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;

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

            ApplyActionWithCooldown(cooldownTime, ApplyModifyMoveSpeedPerk);
        }

        public void ApplyModifyMoveSpeedPerk()
        {
            if (_target == null || !_dstManager.HasComponent<ActorMovementData>(_target.ActorEntity)) return;

            if (_target != Actor.Owner)
            {
                var ownerActorPlayer =
                    Actor.Owner.Abilities.FirstOrDefault(a => a is AbilityActorPlayer) as AbilityActorPlayer;

                if (ownerActorPlayer == null) return;

                this.SetAbilityLevel(ownerActorPlayer.Level, LevelablePropertiesInfoCached, Actor, _target);
                TryUpdateLifespan();
            }

            if (!_target.AppliedPerks.Contains(this)) _target.AppliedPerks.Add(this);

            if (moveFX != null)
            {
                var spawnData = new ActorSpawnerSettings
                {
                    objectsToSpawn = new List<GameObject> {moveFX},
                    SpawnPosition = SpawnPosition.UseSpawnerPosition,
                    parentOfSpawns = TargetType.None,
                    runSpawnActionsOnObjects = true,
                    destroyAbilityAfterSpawn = true
                };

                var fx = ActorSpawn.Spawn(spawnData, Actor, null)?.First();
            }

            var movementData = _dstManager.GetComponentData<ActorMovementData>(_target.ActorEntity);
            movementData.ExternalMultiplier *= moveSpeedMultiplier;

            _dstManager.SetComponentData(_target.ActorEntity, movementData);

            if (!limitedLifespan) return;

            Timer.TimedActions.AddAction(FinishModifiedMoveSpeedTimer, lifespan);
        }

        public void SetLevel(int level)
        {
            this.SetAbilityLevel(level, LevelablePropertiesInfoCached, Actor);
        }

        public void Remove()
        {
            foreach (var component in perkRelatedComponents)
            {
                Destroy(component);
            }

            FinishModifiedMoveSpeedTimer();
            FinishTimer();

            if (Actor != _target) Actor.GameObject.DestroyWithEntity(Actor.ActorEntity);
            if (moveFX != null) Destroy(moveFX);

            Destroy(this);
        }

        private void FinishModifiedMoveSpeedTimer()
        {
            if (_target == null || !_dstManager.HasComponent<ActorMovementData>(_target.ActorEntity) ||
                moveSpeedMultiplier <= 0) return;

            if (_target.AppliedPerks.Contains(this)) _target.AppliedPerks.Remove(this);

            var movementData = _dstManager.GetComponentData<ActorMovementData>(_target.ActorEntity);

            movementData.ExternalMultiplier /= moveSpeedMultiplier;
            _dstManager.SetComponentData(_target.ActorEntity, movementData);
        }

        private void TryUpdateLifespan()
        {
            var lifespanAbility = Actor.Abilities.FirstOrDefault(a => a is AbilityLifespan) as AbilityLifespan;
            if (lifespanAbility == null) return;
            lifespanAbility.lifespan = lifespan;
            lifespanAbility.Timer.TimedActions.Clear();
            lifespanAbility.Execute();
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


        public void SetLevelableProperty()
        {
            this.SetLevelableProperty(LevelablePropertiesInfoCached);
        }
    }
}