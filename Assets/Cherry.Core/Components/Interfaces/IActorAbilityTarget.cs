using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using Unity.Entities;

namespace GameFramework.Example.Components.Interfaces
{
    public interface IActorAbilityTarget : IActorAbility
    {
        IActor TargetActor { get; set; }
        IActor AbilityOwnerActor { get; set; }
    }
}