using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = System.Random;

namespace GameFramework.Example.Loading
{
    [Serializable]
    public struct ActorSpawnerSettings : IActorSpawnerSettings
    {
        [Space] public bool spawnerDisabled;
        
        [Space] public List<GameObject> objectsToSpawn;

        [EnumToggleButtons] public SpawnPosition spawnPosition;

        [ShowIf("spawnPosition", SpawnPosition.UseSpawnPoints)] [EnumToggleButtons]
        public FillOrder spawnPointsFillingMode;

        public FillMode fillSpawnPoints;

        [ShowIf("fillSpawnPoints", FillMode.PlaceEachObjectXTimes)]
        public int x;

        [ShowIf("spawnPosition", SpawnPosition.UseSpawnPoints)]
        public bool skipBusySpawnPoints;

        [ShowIf("spawnPosition", SpawnPosition.UseSpawnPoints)] [EnumToggleButtons]
        public SpawnPointsSource spawnPointsFrom;

        [ShowIf("spawnPosition", SpawnPosition.UseSpawnPoints)]
        [ShowIf("SpawnPointsFrom", SpawnPointsSource.Manually)]
        [SceneObjectsOnly]
        public List<GameObject> spawnPoints;

        [ShowIf("spawnPosition", SpawnPosition.UseSpawnPoints)]
        [ShowIf("SpawnPointsFrom", SpawnPointsSource.FindByTag)]
        [ValueDropdown("Tags")]
        public string spawnPointTag;

        public RotationOfSpawns rotationOfSpawns;

        [EnumToggleButtons] public TargetType parentOfSpawns;

        [ShowIf("parentOfSpawns", TargetType.ComponentName)]
        public string actorWithComponentName;

        [ShowIf("parentOfSpawns", TargetType.ChooseByTag)] [ValueDropdown("Tags")]
        public string parentTag;

        [HideIf("@parentOfSpawns == TargetType.Spawner || parentOfSpawns == TargetType.None")] [EnumToggleButtons]
        public ChooseTargetStrategy chooseParentStrategy;

        public List<GameObject> copyComponentsFromSamples;

        [ShowIf("@copyComponentsFromSamples.Count > 0")]
        [InfoBox(
            "Transforms and IActors will not be copied in any case!\n Only components of chosen type will be replaced!")]
        public ComponentsOfType copyComponentsOfType;

        [ShowIf("@copyComponentsFromSamples.Count > 0")]
        public bool deleteExistingComponents;

        public bool runSpawnActionsOnObjects;
        public bool destroyAbilityAfterSpawn;
        public int randomSeed;

        private System.Random _rnd;

        public bool SpawnerDisabled
        {
            get => spawnerDisabled;
            set => spawnerDisabled = value;
        }

        public List<GameObject> ObjectsToSpawn
        {
            get => objectsToSpawn ?? (objectsToSpawn = new List<GameObject>());
            set => objectsToSpawn = value;
        }

        public SpawnPosition SpawnPosition
        {
            get => spawnPosition;
            set => spawnPosition = value;
        }

        public FillOrder SpawnPointsFillingMode
        {
            get => spawnPointsFillingMode;
            set => spawnPointsFillingMode = value;
        }

        public FillMode FillSpawnPoints
        {
            get => fillSpawnPoints;
            set => fillSpawnPoints = value;
        }

        public int X
        {
            get => x;
            set => x = value;
        }

        public bool SkipBusySpawnPoints
        {
            get => skipBusySpawnPoints;
            set => skipBusySpawnPoints = value;
        }

        public List<GameObject> SpawnPoints
        {
            get
            {
                if (spawnPoints == null) spawnPoints = new List<GameObject>();
                spawnPoints.RemoveAll(go => go == null);

                return spawnPoints;
            }
            set => spawnPoints = value;
        }

        public SpawnPointsSource SpawnPointsFrom
        {
            get => spawnPointsFrom;
            set => spawnPointsFrom = value;
        }

        public string SpawnPointTag
        {
            get => spawnPointTag;
            set => spawnPointTag = value;
        }

        public RotationOfSpawns RotationOfSpawns
        {
            get => rotationOfSpawns;
            set => rotationOfSpawns = value;
        }

        public TargetType ParentOfSpawns
        {
            get => parentOfSpawns;
            set => parentOfSpawns = value;
        }

        public string ActorWithComponentName
        {
            get => actorWithComponentName;
            set => actorWithComponentName = value;
        }

        public string ParentTag
        {
            get => parentTag;
            set => parentTag = value;
        }

        public ChooseTargetStrategy ChooseStrategy
        {
            get => chooseParentStrategy;
            set => chooseParentStrategy = value;
        }


        public bool RunSpawnActionsOnObjects
        {
            get => runSpawnActionsOnObjects;
            set => runSpawnActionsOnObjects = value;
        }

        public int RandomSeed
        {
            get => randomSeed;
            set => randomSeed = value;
        }

        public bool DestroyAbilityAfterSpawn
        {
            get => !SpawnerDisabled && destroyAbilityAfterSpawn;
            set => destroyAbilityAfterSpawn = value;
        }

        public Random Rnd
        {
            get
            {
                if (_rnd != null)
                {
                    return _rnd;
                }

                if (randomSeed == default)
                {
                    return _rnd = new System.Random();
                }

                return _rnd = new System.Random(randomSeed);
            }
        }

        public List<GameObject> CopyComponentsFromSamples
        {
            get
            {
                if (copyComponentsFromSamples == null) copyComponentsFromSamples = new List<GameObject>();
                copyComponentsFromSamples.RemoveAll(go => go == null);

                return copyComponentsFromSamples;
            }
            set => copyComponentsFromSamples = value;
        }

        public ComponentsOfType CopyComponentsOfType
        {
            get => copyComponentsOfType;
            set => copyComponentsOfType = value;
        }

        public bool DeleteExistingComponents
        {
            get => deleteExistingComponents;
            set => deleteExistingComponents = value;
        }


        private static IEnumerable Tags()
        {
            return EditorUtils.GetEditorTags();
        }
    }
}