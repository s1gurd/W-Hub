using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Object = UnityEngine.Object;

namespace GameFramework.Example.Serialization
{
    [Serializable]
    public sealed class PrefabDictionary : ISerializationCallbackReceiver
    {
        public List<PrefabItem> items;

        public Dictionary<ushort, PrefabItem> UShortDictionary { get; } = new Dictionary<ushort, PrefabItem>();

        public Dictionary<string, PrefabItem> StringDictionary { get; } = new Dictionary<string, PrefabItem>();

        public static implicit operator PrefabDictionary(List<Object> prefabs)
        {
            return new PrefabDictionary
            {
                items = prefabs.Select(asset => new PrefabItem(asset)).ToList()
            };
        }

        public void Add(Object prefab)
        {
            items.Add(new PrefabItem(prefab));
        }

        public void Clear()
        {
            items.Clear();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (items != null)
            {
                UShortDictionary.Clear();
                StringDictionary.Clear();

                foreach (var item in items)
                {
                    if (UShortDictionary.TryGetValue(item.id, out var otherItem))
                    {
                        Debug.LogWarning($"Prefab short hash collision: hash = {item.id}, " +
                                         $"item = {item.name}, " +
                                         $"otherItem = {otherItem.name}");
                    }
                    else
                    {
                        UShortDictionary.Add(item.id, item);
                    }

                    StringDictionary.Add(item.name, item);
                }
            }
        }
    }
}