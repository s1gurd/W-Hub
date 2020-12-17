using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Common.Interfaces
{
    public interface IPerkAbility
    {
        DuplicateHandlingProperties DuplicateHandlingProperties { get; set; }
        void Apply(IActor target);
        void Remove();
    }

    [Serializable] [Title("Duplicates Handling")]
    public struct DuplicateHandlingProperties
    {
        public bool HandlingDuplicates;
        
        [EnumToggleButtons][ShowIf("HandlingDuplicates")]
        public DuplicateHandling HandlingType;
        [ShowIf("HandlingDuplicates")]
        public string ComponentName;
    }
    
    public enum DuplicateHandling
    {
        Abort = 0,
        Replace = 1
    }
}