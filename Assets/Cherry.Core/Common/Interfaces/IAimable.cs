using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Common.Interfaces
{
    public interface IAimable
    {
        bool AimingAvailable { get; set; }
        bool DeactivateAimingOnCooldown { get; set; }
        bool OnHoldAttackActive { get; set; }
        bool ActionExecutionAllowed { get; set; }
        GameObject SpawnedAimingPrefab { get; set; }
        AimingProperties AimingProperties { get; set; }
        AimingAnimationProperties AimingAnimProperties { get; set; }
        void EvaluateAim(Vector2 pos);
        void EvaluateAimBySelectedType(Vector2 pos);
        void ResetAiming();
    }
    
    public struct AimingAnimProperties : IComponentData
    {
        public int AnimHash;
    }

    public struct ActorEvaluateAimingAnimData : IComponentData
    {
        public bool AimingActive;
    }
}