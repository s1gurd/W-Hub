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
    public class PerkRotatingWeapons : CooldownBehaviour, IActorAbility, IPerkAbility, IPerkAbilityBindable, ILevelable, ICooldownable
    {
        [ReadOnly] public int perkLevel = 1;

        public List<MonoBehaviour> perkRelatedComponents = new List<MonoBehaviour>();

        [Space] [TitleGroup("Levelable properties")] [OnValueChanged("SetLevelableProperty")]
        public List<LevelableProperties> levelablePropertiesList = new List<LevelableProperties>();
        
        public float cooldownTime;
        
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
        
        public float CooldownTime
        {
            get => cooldownTime;
            set => cooldownTime = value;
        }
        
        public int BindingIndex { get; set; } = -1;
        
        public DuplicateHandlingProperties DuplicateHandlingProperties
        {
            get => duplicateHandlingProperties;
            set => duplicateHandlingProperties = value;
        }

        private List<FieldInfo> _levelablePropertiesInfoCached = new List<FieldInfo>();

        public IActor Actor { get; set; }

        private IActor _target;

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
            
            if (!Actor.Abilities.Contains(this)) Actor.Abilities.Add(this);
        }

        public void Execute()
        {
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


        public void SetLevelableProperty()
        {
            this.SetLevelableProperty(LevelablePropertiesInfoCached);
        }
    }
}