using System;
using Sirenix.OdinInspector;

namespace GameFramework.Example.Common
{
    [Serializable]
    public class ActorDeathAnimProperties
    {
        [ToggleLeft] public bool HasActorDeathAnimation = false;

        [ShowIf("@HasActorDeathAnimation == true")]
        public string ActorDeathAnimationName;
    }
    
    [Serializable]
    public class ActorTakeDamageAnimProperties
    {
        [ToggleLeft] public bool HasActorTakeDamageAnimation = false;

        [ShowIf("@HasActorTakeDamageAnimation == true")]
        public string ActorTakeDamageName;
    }
}