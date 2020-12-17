using System;
using System.Collections.Generic;
using System.Linq;


namespace GameFramework.Example.Loading
{
    [Serializable]
    //public sealed class DummyConfigService : IConfigService
    public sealed class DummyConfigService 
    {/*
        public CharacterConfig[] characterConfigs;
        public ArmorConfig[] armorConfigs;
        public WeaponConfig[] weaponConfigs;
        public RingConfig[] ringConfigs;

        public Dictionary<string, EntityWithId> RawConfigs => throw new NotImplementedException();

        EntityWithId IConfigService.Get(string id)
        {
            throw new NotImplementedException();
        }

        T IConfigService.Get<T>(string id)
        {
            var allConfigs = ConcatConfigs(characterConfigs, armorConfigs, weaponConfigs, ringConfigs);
            return GetItemConfig<T>(id, allConfigs);
        }

        Dictionary<string, EntityWithId> IConfigService.GetRaw()
        {
            throw new NotImplementedException();
        }

        List<T> IConfigService.GetAllTypeCast<T>()
        {
            throw new NotImplementedException();
        }

        bool IConfigService.Has<T>(string id)
        {
            throw new NotImplementedException();
        }

        T IConfigService.GetUnsafe<T>(string id)
        {
            throw new NotImplementedException();
        }

        List<EntityWithId> IConfigService.GetAll<T>()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<EntityWithId> ConcatConfigs(params IEnumerable<EntityWithId>[] configCollections)
        {
            return configCollections.SelectMany(cfg => cfg);
        }

        private T GetItemConfig<T>(string id, IEnumerable<EntityWithId> configs)
            where T : EntityWithId
        {
            var foundItemConfig = configs.FirstOrDefault(cfg => cfg.Id == id) as T;

            if (foundItemConfig == null)
            {
                throw new KeyNotFoundException($"id: {id}, type: {typeof(T).Name}");
            }

            return foundItemConfig;
        }*/
    }
}