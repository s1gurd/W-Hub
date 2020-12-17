using System.Collections.Generic;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components;
using GameFramework.Example.Enums;
using GameFramework.Example.Loading;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Common
{
    [HideMonoScript]
    public class PerkUpgradeBase : MonoBehaviour, IPerkUpgrade
    {
        public string perkName;
        public Sprite perkImage;
        public GameObject perkPrefab;
        public int perkWeight;
        public int availabilityLevel;
        public int availableLevelsNumber;

        public string PerkName => perkName;

        public Sprite PerkImage => perkImage;

        public GameObject PerkPrefab => perkPrefab;
        public float PerkWeight => perkWeight;
        public int AvailabilityLevel => availabilityLevel;
        public int AvailableLevelsNumber => availableLevelsNumber;

        public void SpawnPerk(IActor target)
        {
            var spawn = target.GameObject.AddComponent<AbilityActorSpawn>();
            var perkData = new ActorSpawnerSettings
            {
                objectsToSpawn = new List<GameObject> {perkPrefab},
                SpawnPosition = SpawnPosition.UseSpawnerPosition,
                parentOfSpawns = TargetType.None,
                runSpawnActionsOnObjects = true,
                destroyAbilityAfterSpawn = true
            };
            spawn.SpawnData = perkData;
            var targetActorEntity = target.ActorEntity;
            spawn.AddComponentData(ref targetActorEntity,target);
            spawn.Execute();
        }
    }
}