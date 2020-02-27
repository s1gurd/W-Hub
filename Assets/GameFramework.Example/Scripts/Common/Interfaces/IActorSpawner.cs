using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Example.Common
{
    public interface IActorSpawner
    {
        List<GameObject> SpawnedObjects { get; }
        void Spawn();
        void RunSpawnActions();
    }
}