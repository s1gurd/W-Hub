using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Example.Common.Interfaces
{
    public interface IActorSpawnerAbility : IActorSpawner
    {
        List<Action<GameObject>> SpawnCallbacks { get; set; }
        Action<GameObject> DisposableSpawnCallback { get; set; }
    }
}