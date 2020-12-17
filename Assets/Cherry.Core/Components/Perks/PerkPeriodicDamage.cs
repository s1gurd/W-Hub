using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cherry.Core.Components.Interfaces;
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
    public class PerkPeriodicDamage : TimerBaseBehaviour, IActorAbilityTarget, IPerkAbility, ILevelable
    {
        [ReadOnly] public int perkLevel = 1;

        [LevelableValue] public float healthDecrement = 10;
        [LevelableValue] public float applyPeriod = 1;

        public bool limitedLifespan = true;
        
        [ShowIf("limitedLifespan")]
        [LevelableValue] public float lifespan = 5;

        [Space] [TitleGroup("Levelable properties")] [OnValueChanged("SetLevelableProperty")]
        public List<LevelableProperties> levelablePropertiesList = new List<LevelableProperties>();
        
        public DuplicateHandlingProperties duplicateHandlingProperties;

        public IActor Actor { get; set; }
        public IActor TargetActor { get; set; }
        public IActor AbilityOwnerActor { get; set; }

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
        
        public DuplicateHandlingProperties DuplicateHandlingProperties
        {
            get => duplicateHandlingProperties;
            set => duplicateHandlingProperties = value;
        }

        private List<FieldInfo> _levelablePropertiesInfoCached = new List<FieldInfo>();

        private IActor _effectInstance;
        
        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
            
            if (!Actor.Abilities.Contains(this)) Actor.Abilities.Add(this);
        }

        public void Execute()
        {
            if (TargetActor != Actor.Owner)
            {
                var ownerActorPlayer =
                    Actor.Owner.Abilities.FirstOrDefault(a => a is AbilityActorPlayer) as AbilityActorPlayer;

                if (ownerActorPlayer == null) return;

                this.SetAbilityLevel(ownerActorPlayer.Level, LevelablePropertiesInfoCached, Actor, TargetActor);
                TryUpdateLifespan();
            }

            ApplyPeriodicDamage();
            
            if (!limitedLifespan) return;
            
            Timer.TimedActions.AddAction(Remove, lifespan);
        }

        void ApplyPeriodicDamage()
        {
            if (TargetActor == null || Timer == null) return;

            TargetActor.ActorEntity.Damage(AbilityOwnerActor.ActorEntity, healthDecrement);

            Timer.TimedActions.AddAction(ApplyPeriodicDamage, applyPeriod);
        }

        public void Apply(IActor target)
        {
            this.CheckPerkDuplicates(target, out var continuePerkApply);
            
            if (!continuePerkApply) return;
            
            var copy = target.GameObject.CopyComponent(this) as PerkPeriodicDamage;

            if (copy == null)
            {
                Debug.LogError("[PERK PERIODIC DAMAGE] Error copying perk to Actor!");
                return;
            }

            if (!Actor.Spawner.AppliedPerks.Contains(copy)) Actor.Spawner.AppliedPerks.Add(copy);

            copy.AbilityOwnerActor = this.Actor.Owner;
            copy.TargetActor = Actor.Spawner;
            copy._effectInstance = Actor;
            var targetActorEntity = target.ActorEntity;
            copy.AddComponentData(ref targetActorEntity, target);
            copy.Execute();
        }

        public void SetLevel(int level)
        {
            this.SetAbilityLevel(level, LevelablePropertiesInfoCached, Actor);
        }

        public void Remove()
        {
            if (Actor != null && Actor.AppliedPerks.Contains(this))
            {
                Actor.AppliedPerks.Remove(this);
            }
            
            _effectInstance?.GameObject.DestroyWithEntity(_effectInstance.ActorEntity);
            
            if (this == null) return;

            if (this.ContainsAction(ApplyPeriodicDamage))
            {
                this.RemoveAction(ApplyPeriodicDamage);
            }
            
            Destroy(this);
        }
        
        private void TryUpdateLifespan()
        {
            var lifespanAbility = Actor.Abilities.FirstOrDefault(a => a is AbilityLifespan) as AbilityLifespan;
            if (lifespanAbility == null) return;
            lifespanAbility.lifespan = lifespan;
            lifespanAbility.Timer.TimedActions.Clear();
            lifespanAbility.Execute();
        }


        public void SetLevelableProperty()
        {
            this.SetLevelableProperty(LevelablePropertiesInfoCached);
        }
    }
}