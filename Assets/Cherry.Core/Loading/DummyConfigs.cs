using System.Collections.Generic;
using System.Linq;


using Sirenix.OdinInspector;

using UnityEngine;

namespace GameFramework.Example.Loading
{
    [CreateAssetMenu(fileName = "Dummy Configs", menuName = "Game.Framework/Create Dummy Configs", order = 1)]
    public sealed class DummyConfigs : ScriptableObject
    {/*
        public DummyConfigService configService;
        public TextAsset[] configTables;

        [Button("Load from files")]
        public void LoadFromFiles()
        {
            var csvParser = new CSVParser();

            List<Dictionary<string, string>> allEntries = new List<Dictionary<string, string>>();

            foreach (var configTable in configTables)
            {
                var entries = csvParser.Parse(configTable.text);
                allEntries.AddRange(entries);
            }

            var characterConfigs = csvParser.PopObjects<CharacterConfig>(allEntries);
            var armorConfigs = csvParser.PopObjects<ArmorConfig>(allEntries);
            var weaponConfigs = csvParser.PopObjects<WeaponConfig>(allEntries);
            var ringConfigs = csvParser.PopObjects<RingConfig>(allEntries);

            configService.characterConfigs = characterConfigs.ToArray();
            configService.armorConfigs = armorConfigs.ToArray();
            configService.weaponConfigs = weaponConfigs.ToArray();
            configService.ringConfigs = ringConfigs.ToArray();
        }*/
    }
}