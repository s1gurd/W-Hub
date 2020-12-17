using System;
using Sirenix.OdinInspector;

namespace GameFramework.Example.Common.Interfaces
{
    [Serializable]
    public class AimingAnimationProperties
    {
        [ToggleLeft] public bool HasActorAimingAnimation = false;

        [ShowIf("@HasActorAimingAnimation")]
        public string ActorAimingAnimationName;
    }
}