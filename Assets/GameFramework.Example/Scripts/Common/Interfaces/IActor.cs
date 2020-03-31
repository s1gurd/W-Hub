using System.Collections.Generic;
using GameFramework.Example.Components.Interfaces;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Common.Interfaces
{
    public interface IActor
    {
        Entity ActorEntity { get; }
        EntityManager WorldEntityManager { get; }
        GameObject Spawner { get; set; } 
        List<string> ComponentNames { get; }
        List<IActorAbility> Abilities { get; }
        void PerformSpawnActions();
    }
}