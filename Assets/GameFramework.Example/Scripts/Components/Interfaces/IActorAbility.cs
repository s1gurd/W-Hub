using Unity.Entities;

namespace GameFramework.Example.Components.Interfaces
{
    public interface IActorAbility
    {
        void AddComponentData(ref Entity entity);
        void Execute();
    }
}