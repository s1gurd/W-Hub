using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cherry.Core.Components.Interfaces;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityApplyDamage : MonoBehaviour, IActorAbilityTarget,  ILevelable
    {
        [ReadOnly] public int perkLevel = 1;
        
        [LevelableValue] public float damageValue = 0;
        
        [Space] [TitleGroup("Levelable properties")] [OnValueChanged("SetLevelableProperty")]
        public List<LevelableProperties> levelablePropertiesList = new List<LevelableProperties>();
        public IActor TargetActor { get; set; }
        public IActor AbilityOwnerActor { get; set; }
        public IActor Actor { get; set; }
        
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

        private List<FieldInfo> _levelablePropertiesInfoCached = new List<FieldInfo>();

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
        }

        public void Execute()
        {
            if (TargetActor == null) return;
            
            var ownerActorPlayer =
                AbilityOwnerActor.Abilities.FirstOrDefault(a => a is AbilityActorPlayer) as AbilityActorPlayer;
                
            if (ownerActorPlayer == null) return;
                
            this.SetAbilityLevel(ownerActorPlayer.Level, LevelablePropertiesInfoCached, Actor, TargetActor);
            
            TargetActor.ActorEntity.Damage(AbilityOwnerActor.ActorEntity, damageValue);
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