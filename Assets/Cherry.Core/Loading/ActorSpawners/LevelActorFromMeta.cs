using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components;
using GameFramework.Example.Enums;
using GameFramework.Example.Loading.Interfaces;
using Newtonsoft.Json;
using UnityEngine;

namespace GameFramework.Example.Loading.ActorSpawners
{
    public sealed class LevelActorFromMeta : IActorSpawner
    {/*
        private readonly MatchPlayer _matchPlayer;
        private readonly IConfigService _configService;
        private readonly IPrefabRepository _prefabRepository;
        private readonly IGameObjectRepository _gameObjectRepository;

        public LevelActorFromMeta(
            IPrefabRepository prefabRepository,
            IGameObjectRepository gameObjectRepository,
            IConfigService configService,
            MatchPlayer matchPlayer)
        {
            _matchPlayer = matchPlayer;
            _prefabRepository = prefabRepository;
            _configService = configService;
            _gameObjectRepository = gameObjectRepository;
        }

        public List<GameObject> SpawnedObjects { get; set; }

        public IActorSpawnerSettings ActorSpawnerSettings { get; set; }

        void IActorSpawner.Spawn()
        {
            ActorSpawnerSettings = CreateActorSpawnerSettings(_matchPlayer);
            SpawnedObjects = ActorSpawn.Spawn(ActorSpawnerSettings);
        }

        void IActorSpawner.RunSpawnActions()
        {
            _ = ActorSpawn.RunSpawnActions(SpawnedObjects);
            var abilityActorPlayer = SpawnedObjects.First().GetComponent<AbilityActorPlayer>();
            ActorSpawn.FillPlayerData(abilityActorPlayer, _matchPlayer, _configService);
        }

        private IActorSpawnerSettings CreateActorSpawnerSettings(MatchPlayer matchPlayer)
        {
            var itemConfigs = matchPlayer.Slots.Select(_configService.Get<ItemConfig>).ToArray();
            var characterConfig = _configService.Get<CharacterConfig>(matchPlayer.CharacterId);

            Debug.Log(
                $"CreateActorSpawnerSettings: itemConfigs = {JsonConvert.SerializeObject(itemConfigs, Formatting.Indented)}");
            Debug.Log(
                $"CreateActorSpawnerSettings: characterConfig = {JsonConvert.SerializeObject(characterConfig, Formatting.Indented)}");

            var prefabName = characterConfig.IngamePrefab; // LooserIngame

            // TODO: Move perks data assigning to a proper place.
            var characterPerks = characterConfig.PerkList.Select(_configService.Get<PerkConfig>).ToArray();

            foreach (var itemConfig in itemConfigs)
            {
                switch (itemConfig)
                {
                    case ArmorConfig armorConfig:
                        var moveSpeed = armorConfig.MoveSpeed; // 240
                        var armorHealth = armorConfig.Health; // 100
                        var armorPerks = armorConfig.PerkList.Select(_configService.Get<PerkConfig>).ToArray();
                        break;

                    case WeaponConfig weaponConfig:
                        var fireRate = weaponConfig.FireRate; // 0.0f
                        var critChance = weaponConfig.CritChance; // 0.1f
                        var critMultiplier = weaponConfig.CritMultiplier; // 1.25f
                        //var weaponPerks = weaponConfig.PerkList.Select(_configService.Get<PerkConfig>).ToArray(); // [ "IdDouble_throw" ]

                        //Debug.Log($"CreateActorSpawnerSettings: weaponPerks = {JsonConvert.SerializeObject(weaponPerks, Formatting.Indented)}");
                        break;

                    case RingConfig ringConfig:
                        var ringHealth = ringConfig.Health; // 30
                        var ringPerks = ringConfig.PerkList.Select(_configService.Get<PerkConfig>).ToArray();
                        break;
                }
            }

            var prefab = _prefabRepository.Get<GameObject>(prefabName);
            var spawnPoints = GetSpawnPoints(matchPlayer);
            var spawnPositionType = GetSpawnPositionType(matchPlayer);
            var spawnRotationType = GetSpawnRotationType(matchPlayer);
            var spawnParentType = GetSpawnParentType(matchPlayer);
            var spawnParentStrategy = GetSpawnParentStrategy(matchPlayer);
            var useSpawnActionsOnObjects = CheckNeedSpawnActionsOnObjects(matchPlayer);
            var sampleGeneral = _prefabRepository.Get<GameObject>("CharacterSampleGeneral");
            var samplePlayer = GetSample(matchPlayer);

            return new ActorSpawnerSettings
            {
                ObjectsToSpawn = new List<GameObject> {prefab},
                SpawnPosition = spawnPositionType,
                SpawnPointsFillingMode = FillOrder.SequentialOrder,
                FillSpawnPoints = FillMode.UseEachObjectOnce,
                SkipBusySpawnPoints = true,
                SpawnPoints = spawnPoints,
                RotationOfSpawns = spawnRotationType,
                ParentOfSpawns = spawnParentType,
                chooseParentStrategy = spawnParentStrategy,
                RunSpawnActionsOnObjects = useSpawnActionsOnObjects,
                DestroyAbilityAfterSpawn = false,
                CopyComponentsFromSamples = new List<GameObject> {sampleGeneral, samplePlayer},
                CopyComponentsOfType = ComponentsOfType.AllComponents
            };
        }

        private List<GameObject> GetSpawnPoints(MatchPlayer matchPlayer)
        {
            var result = new List<GameObject>();

            result.Add(_gameObjectRepository.Get<GameObject>("point (25)"));
            result.Add(_gameObjectRepository.Get<GameObject>("point (23)"));

            return result;
        }

        private SpawnPosition GetSpawnPositionType(MatchPlayer matchPlayer)
        {
            switch (matchPlayer.PlayerType)
            {
                case PlayerType.Network: // Not sure if that's right.
                    return SpawnPosition.UseSpawnPoints;

                case PlayerType.Bot:
                case PlayerType.Local:
                case PlayerType.NetworkReplacedByBot:
                    return SpawnPosition.RandomPositionOnNavMesh;

                default:
                    throw new System.NotImplementedException(
                        $"Cannot get spawn position type for PlayerType = {matchPlayer.PlayerType}");
            }
        }

        private RotationOfSpawns GetSpawnRotationType(MatchPlayer matchPlayer)
        {
            switch (matchPlayer.PlayerType)
            {
                case PlayerType.Local:
                case PlayerType.Network: // Not sure if that's right.
                    return RotationOfSpawns.UseSpawnPointRotation;

                case PlayerType.Bot:
                    return RotationOfSpawns.UseRandomRotationY;

                default:
                    throw new System.NotImplementedException(
                        $"Cannot get spawn rotation type for PlayerType = {matchPlayer.PlayerType}");
            }
        }

        private TargetType GetSpawnParentType(MatchPlayer matchPlayer)
        {
            switch (matchPlayer.PlayerType)
            {
                case PlayerType.Local:
                case PlayerType.Network: // Not sure if that's right.
                    return TargetType.None;

                case PlayerType.Bot:
                    return TargetType.ComponentName;

                default:
                    throw new System.NotImplementedException(
                        $"Cannot get spawn parent type for PlayerType = {matchPlayer.PlayerType}");
            }
        }

        private ChooseTargetStrategy GetSpawnParentStrategy(MatchPlayer matchPlayer)
        {
            switch (matchPlayer.PlayerType)
            {
                case PlayerType.Local:
                case PlayerType.Network: // Not sure if that's right.
                    return default;

                case PlayerType.Bot:
                    return ChooseTargetStrategy.Nearest;

                default:
                    throw new System.NotImplementedException(
                        $"Cannot get spawn parent strategy for PlayerType = {matchPlayer.PlayerType}");
            }
        }

        private bool CheckNeedSpawnActionsOnObjects(MatchPlayer matchPlayer)
        {
            switch (matchPlayer.PlayerType)
            {
                case PlayerType.Local:
                case PlayerType.Network: // Not sure if that's right.
                    return true;

                case PlayerType.Bot:
                    return false;

                default:
                    throw new System.NotImplementedException(
                        $"Cannot get spawn action usage flag for PlayerType = {matchPlayer.PlayerType}");
            }
        }

        private GameObject GetSample(MatchPlayer matchPlayer)
        {
            switch (matchPlayer.PlayerType)
            {
                case PlayerType.Local:
                    return _prefabRepository.Get<GameObject>("CharacterSampleUser");
                case PlayerType.Network: // Not sure if that's right.
                    return _prefabRepository.Get<GameObject>("CharacterSampleNetwork");

                default:
                    return _prefabRepository.Get<GameObject>("CharacterSampleBot");
            }
        }*/
        public List<GameObject> SpawnedObjects { get; }
        public void Spawn()
        {
            throw new System.NotImplementedException();
        }

        public void RunSpawnActions()
        {
            throw new System.NotImplementedException();
        }
    }
}