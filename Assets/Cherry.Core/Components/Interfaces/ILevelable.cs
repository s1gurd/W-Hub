using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Cherry.Core.Components.Interfaces
{
    public interface ILevelable
    {
        int Level { get; set; }
        void SetLevel(int level);
        void SetLevelableProperty();
        List<LevelableProperties> LevelablePropertiesList { get; set; }
    }
    
    [Serializable]
    public struct LevelableProperties
    {
        [ValueDropdown("levelablePropertiesInfo")]
        public string propertyName;

        public ModifiablePropertiesActions levelablePropertyAction;

        public float modifier;
        
        [HideInInspector] public List<string> levelablePropertiesInfo;
    }

    public enum ModifiablePropertiesActions
    {
        Multiply = 0,
        Add = 1
    }
}