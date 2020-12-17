using System.Collections.Generic;
using System.Linq;

using GameFramework.Example.Common;
using GameFramework.Example.Loading.Interfaces;
using GameFramework.Example.Serialization;

using Sirenix.OdinInspector;

using UnityEngine;

using Object = UnityEngine.Object;

namespace GameFramework.Example.Loading.Repositories
{
    [CreateAssetMenu(fileName = "Prefab Repository", menuName = "Game.Framework/Create Prefab Repository", order = 1)]
    public sealed class PrefabRepositoryFromScriptableObject : ScriptableObject, IPrefabRepository
    {
        public PrefabDictionary items = new PrefabDictionary();

#if UNITY_EDITOR
        [Button]
        public void Fill()
        {
            items = UnityEditor.AssetDatabase.FindAssets("t:Prefab").Select(UnityEditor.AssetDatabase.GUIDToAssetPath)
                                                                    .Select(UnityEditor.AssetDatabase.LoadAssetAtPath<Object>)
                                                                    .ToList();

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        T IPrefabRepository.Get<T>(string name)
        {
            if (items.StringDictionary.TryGetValue(name, out var item))
            {
                return (T)item.asset;
            }

            throw new KeyNotFoundException($"Prefab with name: {name}, not found at prefab repository!");
        }

        T IPrefabRepository.Get<T>(ushort key)
        {
            if (items.UShortDictionary.TryGetValue(key, out var item))
            {
                return (T)item.asset;
            }

            throw new KeyNotFoundException($"Prefab with key '{key}' not found at prefab repository!");
        }
    }
}