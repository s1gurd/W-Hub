using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
using JetBrains.Annotations;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;


namespace GameFramework.Example.Common
{
    public class ActorSpawn
    {
        [CanBeNull]
        public static List<GameObject> Spawn(IActorSpawnerSettings spawnSettings, IActor spawner = null,
            IActor owner = null)
        {
            if (spawnSettings.SpawnerDisabled) return null;
            
            Vector3 tempPos = Vector3.zero;
            Quaternion tempRot = Quaternion.Euler(Vector3.zero);
            GameObject tempObj;

            int spawnCount;

            List<Component> sampledComponents = new List<Component>();
            List<GameObject> spawnedObjects = new List<GameObject>();

            if (spawnSettings.SpawnPosition == SpawnPosition.UseSpawnPoints &&
                spawnSettings.SpawnPointsFrom == SpawnPointsSource.FindByTag)
            {
                spawnSettings.SpawnPoints = GameObject.FindGameObjectsWithTag(spawnSettings.SpawnPointTag).ToList();
                if (spawnSettings.SpawnPoints.Count == 0)
                {
                    Debug.LogError("[LEVEL ACTOR SPAWNER] No spawn points found with tag: " +
                                   spawnSettings.SpawnPointTag + ". Aborting!");
                    return null;
                }
            }
            
            switch (spawnSettings.FillSpawnPoints)
            {
                case FillMode.UseEachObjectOnce:
                    spawnCount = spawnSettings.ObjectsToSpawn.Count;
                    break;
                case FillMode.FillAllSpawnPoints:
                    spawnCount = spawnSettings.SpawnPoints.Count;
                    break;
                case FillMode.PlaceEachObjectXTimes:
                    spawnCount = spawnSettings.ObjectsToSpawn.Count * spawnSettings.X;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (spawnSettings.SkipBusySpawnPoints && spawnSettings.SpawnPosition == SpawnPosition.UseSpawnPoints)
            {
                for (var i = 0; i < spawnSettings.SpawnPoints.Count; i++)
                {
                    var actorSpawnedOnPoint = spawnSettings.SpawnPoints[i].GetComponent<ActorSpawnedOnPoint>();

                    if (actorSpawnedOnPoint != null && actorSpawnedOnPoint.actor != null)
                    {
                        spawnSettings.SpawnPoints.RemoveAt(i);
                        spawnCount--;
                        i--;
                    }
                }
            }

            if (spawnSettings.SpawnPointsFillingMode == FillOrder.RandomOrder)
            {
                spawnSettings.ObjectsToSpawn =
                    spawnSettings.ObjectsToSpawn.OrderBy(item => spawnSettings.Rnd.Next()).ToList();
                spawnSettings.SpawnPoints =
                    spawnSettings.SpawnPoints.OrderBy(item => spawnSettings.Rnd.Next()).ToList();
            }

            for (var i = 0; i < spawnCount; i++)
            {
                switch (spawnSettings.SpawnPosition)
                {
                    case SpawnPosition.UseSpawnPoints:
                    {
                        if (spawnSettings.SpawnPoints.Count == 0)
                        {
                            Debug.LogError(
                                $"[ACTOR SPAWNER] In Use Spawn Points mode you have to provide some spawning points! \n" +
                                $"Spawner is {spawner}, and object is {spawnSettings.ObjectsToSpawn[0]}");
                            return null;
                        }

                        tempPos = spawnSettings.SpawnPoints[i % spawnSettings.SpawnPoints.Count].transform.position;
                        if (spawnSettings.RotationOfSpawns == RotationOfSpawns.UseSpawnPointRotation)
                        {
                            tempRot = spawnSettings.SpawnPoints[i % spawnSettings.SpawnPoints.Count].transform.rotation;
                        }

                        break;
                    }

                    case SpawnPosition.RandomPositionOnNavMesh:
                        tempPos = NavMeshRandomPointUtil.GetRandomLocation();
                        break;
                    case SpawnPosition.UseSpawnerPosition:
                        if (spawner == null)
                        {
                            Debug.LogError("[ACTOR SPAWNER] You are using Use Spawner Position, but Spawner is NULL!");
                            return null;
                        }

                        if (spawner.GameObject == null) return null;

                        tempPos = spawner.GameObject.transform.position;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (spawnSettings.RotationOfSpawns)
                {
                    case RotationOfSpawns.UseRandomRotationY:
                        tempRot = Quaternion.Euler(0f, spawnSettings.Rnd.Next() % 360f, 0f);
                        break;
                    case RotationOfSpawns.UseRandomRotationXYZ:
                        tempRot = Quaternion.Euler(spawnSettings.Rnd.Next() % 360f, spawnSettings.Rnd.Next() % 360f,
                            spawnSettings.Rnd.Next() % 360f);
                        break;
                    case RotationOfSpawns.UseSpawnPointRotation:
                        if (spawnSettings.SpawnPosition == SpawnPosition.UseSpawnerPosition)
                        {
                            tempRot = spawner.GameObject.transform.rotation;
                        }

                        break;
                    case RotationOfSpawns.UseZeroRotation:
                        tempRot = Quaternion.Euler(Vector3.zero);
                        break;
                    case RotationOfSpawns.SpawnerMovement:
                        try
                        {
                            var spawnerMovementData =
                                World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<ActorMovementData>(
                                    spawner.ActorEntity);
                            tempRot = spawnerMovementData.Input.Equals(float3.zero)?Quaternion.Euler(Vector3.zero): Quaternion.LookRotation(Vector3.Normalize(spawnerMovementData.Input));
                        }
                        catch
                        {
                            Debug.LogError(
                                "[ACTOR SPAWNER] To get Rotation from Spawner Movement, you need IActor Spawner and ActorMovementData on it!");
                            tempRot = Quaternion.Euler(Vector3.zero);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (spawnSettings.CopyComponentsFromSamples.Count > 0)
                {
                    sampledComponents = new List<Component>();

                    foreach (var sample in spawnSettings.CopyComponentsFromSamples)
                    {
                        if (sample == null) continue;

                        foreach (var component in sample.GetComponents<Component>())
                        {
                            if (component == null || component is Transform || component is IActor) continue;
                            switch (spawnSettings.CopyComponentsOfType)
                            {
                                case ComponentsOfType.OnlyAbilities when !(component is IActorAbility):
                                    continue;
                                case ComponentsOfType.OnlySimpleBehaviours when component is IActorAbility:
                                    continue;
                                case ComponentsOfType.AllComponents:
                                default:
                                    sampledComponents.Add(component);
                                    break;
                            }
                        }
                    }

                    if (sampledComponents.Count == 0)
                        Debug.LogError("[LEVEL ACTOR SPAWNER] No suitable components found in sample game objects!");
                }

                tempObj = UnityEngine.Object.Instantiate(
                    spawnSettings.ObjectsToSpawn[i % spawnSettings.ObjectsToSpawn.Count], tempPos, tempRot);

                var actors = tempObj.GetComponents<IActor>();

                if (spawnSettings.ParentOfSpawns != TargetType.None)
                {
                    var parents = FindActorsUtils.GetActorsList(tempObj, spawnSettings.ParentOfSpawns,
                        spawnSettings.ActorWithComponentName, spawnSettings.ParentTag);

                    var parent = FindActorsUtils.ChooseActor(tempObj.transform, parents, spawnSettings.ChooseStrategy);

                    tempObj.transform.SetParent(parent);
                }

                if (sampledComponents.Count > 0)
                {
                    if (spawnSettings.DeleteExistingComponents)
                    {
                        foreach (var component in tempObj.GetComponents<Component>())
                        {
                            if (component is Transform || component is IActor) continue;
                            Object.Destroy(component);
                        }
                    }

                    tempObj.CopyComponentsWithLinks(sampledComponents);
                }

                if (actors.Length > 1)
                {
                    Debug.LogError("[ACTOR SPAWNER] Only one IActor Component for Actor allowed!");
                }
                else if (actors.Length == 1)
                {
                    if (spawner != null)
                    {
                        spawner.ChildrenSpawned++;
                        actors.First().ActorId = spawner.ActorId;
                    }

                    actors.First().Spawner = spawner;
                    actors.First().Owner = owner ?? spawner ?? actors.First();
                    actors.First().Setup();
                }

                if (spawnSettings.SpawnPosition == SpawnPosition.UseSpawnPoints && spawnSettings.SkipBusySpawnPoints)
                {
                    spawnSettings.SpawnPoints[i % spawnSettings.SpawnPoints.Count].AddComponent<ActorSpawnedOnPoint>()
                        .actor = tempObj;
                }

                spawnedObjects.Add(tempObj);
            }

            return spawnedObjects;
        }

        public static List<IActor> RunSpawnActions(IEnumerable<GameObject> objects)
        {
            if (objects == null) return null;

            var objectList = objects.ToList();

            if (!objectList.Any()) return null;

            var actors = objectList.Select(o => o.GetComponent<IActor>()).Where(actorComponent => actorComponent != null)
                .ToList();

            foreach (var a in actors)
            {
                a.PerformSpawnActions();
            }

            return actors;
        }
    }
}