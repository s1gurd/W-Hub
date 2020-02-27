using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Loading;
using GameFramework.Example.Utils;
using Unity.Entities;
using UnityEngine;
using Object = UnityEngine.Object;


namespace GameFramework.Example.Common
{
    public class ActorSpawn
    {
        public static List<GameObject> Spawn(IActorSpawnerSettings spawnSettings, GameObject spawner = null)
        {
            Vector3 tempPos = Vector3.zero;
            Quaternion tempRot = Quaternion.Euler(Vector3.zero);
            GameObject tempObj;

            int spawnCount;

            List<Component> sampledComponents = new List<Component>();
            List<GameObject> spawnedObjects = new List<GameObject>();

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
                foreach (var p in spawnSettings.SpawnPoints)
                {
                    if (p.GetComponent<ActorSpawnedOnPoint>() != null)
                    {
                        spawnSettings.SpawnPoints.Remove(p);
                    }
                }
            }

            if (spawnSettings.SpawnPointsFillingMode == FillOrder.RandomOrder)
            {
                spawnSettings.ObjectsToSpawn =
                    spawnSettings.ObjectsToSpawn.OrderBy((item) => spawnSettings.Rnd.Next()).ToList();
                spawnSettings.SpawnPoints =
                    spawnSettings.SpawnPoints.OrderBy((item) => spawnSettings.Rnd.Next()).ToList();
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
                                "[ACTOR SPAWNER] In Use Spawn Points mode you have to provide some spawning points!");
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
                        break;
                    case RotationOfSpawns.UseZeroRotation:
                        tempRot = Quaternion.Euler(Vector3.zero);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (spawnSettings.CopyComponentsFromSamples.Count > 0)
                {
                    foreach (var sample in spawnSettings.CopyComponentsFromSamples)
                    {
                        foreach (var component in sample.GetComponents<Component>())
                        {
                            if (component is Transform) continue;
                            switch (spawnSettings.CopyComponentsOfType)
                            {
                                case ComponentsOfType.OnlyAbilities when !(component is IActorAbility):
                                case ComponentsOfType.OnlySimpleBehaviours when component is IActorAbility:
                                    continue;
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
                if (actors.Length == 0) continue;
                foreach (var actor in actors)
                {
                    actor.Spawner = spawner;
                }

                if (spawnSettings.ParentOfSpawns != TargetType.None)
                {
                    var parents = FindActorsUtils.GetActorsList(tempObj, spawnSettings.ParentOfSpawns,
                        spawnSettings.ActorWithComponentName, spawnSettings.ParentTag);

                    Debug.Log(parents.Count);
                    var parent = FindActorsUtils.ChooseActor(tempObj.transform, parents, spawnSettings.ChooseStrategy);

                    tempObj.transform.parent = parent;
                }


                if (sampledComponents.Count > 0)
                {
                    if (spawnSettings.DeleteExistingComponents)
                    {
                        foreach (var component in tempObj.GetComponents<Component>())
                        {
                            if (component is Transform) continue;
                            Object.Destroy(component);
                        }
                    }

                    foreach (var newComponent in sampledComponents)
                    {
                        tempObj.CopyComponent(newComponent);
                        if (!(newComponent is IActorAbility)) continue;
                        var actor = tempObj.GetComponent<IActor>();
                        var actorEntity = actor.ActorEntity;
                        (newComponent as IActorAbility).AddComponentData(ref actorEntity);
                    }
                }

                if (spawnSettings.SpawnPosition == SpawnPosition.UseSpawnPoints)
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
            List<IActor> actors = new List<IActor>();

            foreach (var o in objects)
            {
                var actorComponents = o.GetComponents<IActor>();
                if (actorComponents.Length != 0)
                {
                    actors.AddRange(actorComponents);
                }
            }

            foreach (var a in actors)
            {
                a.PerformSpawnActions();
            }

            return actors;
        }
    }
}