using System;
using Sirenix.OdinInspector;

namespace GameFramework.Example.Common
{
    [Serializable]
    public class ActorProjectileSpawnAnimProperties
    {
        [ToggleLeft] public bool HasActorProjectileAnimation = false;

        [ShowIf("@HasActorProjectileAnimation == true")]
        public string ActorProjectileAnimationName;
    }
}