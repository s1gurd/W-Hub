using UnityEngine;

namespace GameFramework.Example.Common.Interfaces
{
    public interface IPerkBase
    {
        string PerkName { get; }
        Sprite PerkImage { get; }
        GameObject PerkPrefab { get; }
        
        void SpawnPerk(IActor target);
    }
}