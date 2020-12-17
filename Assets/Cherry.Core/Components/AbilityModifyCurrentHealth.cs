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
    public class AbilityModifyCurrentHealth: MonoBehaviour, IActorAbilityTarget, ILevelable
    {
        [ReadOnly] public int perkLevel = 1;
        [LevelableValue] public float healthModifier = 15;
        public IActor Actor { get; set; }
        public int Level
        {
            get => perkLevel;
            set => perkLevel = value;
        }
        
        public IActor TargetActor { get; set; }
        public IActor AbilityOwnerActor { get; set; }
        
        [Space] [TitleGroup("Levelable properties")] [OnValueChanged("SetLevelableProperty")]
        public List<LevelableProperties> levelablePropertiesList = new List<LevelableProperties>();
        
        public List<FieldInfo> LevelablePropertiesInfoCached
        {
            get
            {
                if (_levelablePropertiesInfoCached.Any()) return _levelablePropertiesInfoCached;
                return _levelablePropertiesInfoCached = this.GetFieldsWithAttributeInfo<LevelableValue>();
            }
        }
        
        public List<LevelableProperties> LevelablePropertiesList
        {
            get => levelablePropertiesList;
            set => levelablePropertiesList = value;
        }
        
        private List<FieldInfo> _levelablePropertiesInfoCached = new List<FieldInfo>();

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
        }

        public void Execute()
        {
            var actor = TargetActor ?? Actor;
            
            var abilityActorPlayer = actor.Abilities.FirstOrDefault(a => a is AbilityActorPlayer) as AbilityActorPlayer;
            
            if (abilityActorPlayer == null) return;
            abilityActorPlayer.UpdateHealthData((int) healthModifier);
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