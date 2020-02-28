using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.Example.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Common
{
    [Serializable]
    public class ActorSpawnerSettings : IActorSpawnerSettings
    {
        [Space]public List<GameObject> objectsToSpawn;

        [EnumToggleButtons] public SpawnPosition spawnPosition;

        [ShowIf("spawnPosition", SpawnPosition.UseSpawnPoints)] [EnumToggleButtons]
        public FillOrder spawnPointsFillingMode;

        public FillMode fillSpawnPoints;

        [ShowIf("fillSpawnPoints", FillMode.PlaceEachObjectXTimes)]
        public int x = 1;

        [ShowIf("spawnPosition", SpawnPosition.UseSpawnPoints)]
        public bool skipBusySpawnPoints = false;

        [ShowIf("spawnPosition", SpawnPosition.UseSpawnPoints)][SceneObjectsOnly]
        public List<GameObject> spawnPoints;

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
        [InfoBox("Transforms will not be copied in any case!\n Only components of chosen type will be replaced!")]
        public ComponentsOfType copyComponentsOfType = ComponentsOfType.OnlyAbilities;
        
        [ShowIf("@copyComponentsFromSamples.Count > 0")]
        public bool deleteExistingComponents = false;
        

        public bool runSpawnActionsOnObjects = true;
        
        //TODO Inject the seed here to make determenisitic Random
        public System.Random rnd = new System.Random();

        public List<GameObject> ObjectsToSpawn
        {
            get => objectsToSpawn;
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
            get => spawnPoints;
            set => spawnPoints = value;
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

        public System.Random Rnd
        {
            get => rnd;
            set => rnd = value;
        }

        public List<GameObject> CopyComponentsFromSamples
        {
            get => copyComponentsFromSamples;
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
#if UNITY_EDITOR
        

        private static IEnumerable Tags()
        {
            return UnityEditorInternal.InternalEditorUtility.tags;
        }
#endif
        
    }
}