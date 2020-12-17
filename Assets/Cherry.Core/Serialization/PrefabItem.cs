using System;

using UnityEngine;

using Object = UnityEngine.Object;

namespace GameFramework.Example.Serialization
{
    [Serializable]
    public sealed class PrefabItem : ISerializationCallbackReceiver
    {
        [Sirenix.OdinInspector.ReadOnly]
        public ushort id;
        public Object asset;

        [HideInInspector]
        public string name;

        public PrefabItem(Object asset)
        {
            this.asset = asset;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (asset == null) 
            {
                return;
            }

            id = HashUtils.GetAssetHashUShort(asset);
            name = asset.name;
#endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() { } 

    }
}