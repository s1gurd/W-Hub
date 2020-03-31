using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;

namespace GameFramework.Example.Components.Interfaces
{
    public interface IActorDataExchange : IActorAbility
    {
        IActor TargetActor { get; set; }
    }
}