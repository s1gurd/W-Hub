using GameFramework.Example.Common.Interfaces;
using Unity.Entities;

namespace GameFramework.Example.Components.Interfaces
{
    public interface IActorAbility
    {
        IActor Actor { get; set; }
        void AddComponentData(ref Entity entity, IActor actor);
        void Execute();
    }
}