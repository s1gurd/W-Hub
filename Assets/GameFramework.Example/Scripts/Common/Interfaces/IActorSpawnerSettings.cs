using System.Collections.Generic;
using GameFramework.Example.Enums;
using UnityEngine;

namespace GameFramework.Example.Common.Interfaces
{
    public interface IActorSpawnerSettings
    {
        List<GameObject> ObjectsToSpawn { get; set; }
        SpawnPosition SpawnPosition { get; set; }
        FillOrder SpawnPointsFillingMode { get; set; }
        FillMode FillSpawnPoints { get; set; }
        int X { get; set; }
        bool SkipBusySpawnPoints { get; set; }
        List<GameObject> SpawnPoints { get; set; }
        RotationOfSpawns RotationOfSpawns { get; set; }
        TargetType ParentOfSpawns { get; set; }
        string ActorWithComponentName { get; set; }
        string ParentTag { get; set; }
        ChooseTargetStrategy ChooseStrategy { get; set; }
        bool RunSpawnActionsOnObjects { get; set; }
        System.Random Rnd { get; set; }
        List<GameObject> CopyComponentsFromSamples { get; set; }
        ComponentsOfType CopyComponentsOfType { get; set; }
        bool DeleteExistingComponents { get; set; }
        
    }
}