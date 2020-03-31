using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Example.Common.Interfaces
{
    public interface IActorSpawner
    {
        List<GameObject> SpawnedObjects { get; }
        void Spawn();
        void RunSpawnActions();
    }
}