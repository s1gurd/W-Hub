using UnityEngine;

namespace GameFramework.Example.Common.Interfaces
{
    public interface IPerkAbilityForSpawned : IPerkAbility
    {
        void AddCollisionAction(GameObject target);
    }
}