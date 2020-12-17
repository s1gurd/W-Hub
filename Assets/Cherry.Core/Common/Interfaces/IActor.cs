using System.Collections.Generic;
using GameFramework.Example.Components;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Components.Interfaces;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Common.Interfaces
{
    public interface IActor
    {
        Entity ActorEntity { get; }
        EntityManager WorldEntityManager { get; }
        IActor Spawner { get; set; }
        IActor Owner { get; set; }
        int ActorId { get; set; }
        int ActorStateId { get;  }
        ushort ChildrenSpawned { get; set; }
        
        GameObject GameObject { get; }
        List<string> ComponentNames { get; }
        List<IActorAbility> Abilities { get; }
        List<IPerkAbility> AppliedPerks { get; }
        void PerformSpawnActions();
        void Setup();
    }
}