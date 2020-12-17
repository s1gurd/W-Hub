using System;
using System.Text;

using Object = UnityEngine.Object;

namespace GameFramework.Example.Serialization
{
    public static class HashUtils
    {
#if UNITY_EDITOR
        public static ushort GetAssetHashUShort(Object prefab)
        {
            if (UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(prefab, out string guid, out long _))
            {
                ushort result = 0;

                var bytes = Encoding.UTF8.GetBytes(guid);

                for (int i = 1; i < bytes.Length; i++)
                {
                    result ^= BitConverter.ToUInt16(new [] { bytes[i - 1], bytes[i] }, 0);
                }

                return result;
            }

            throw new InvalidOperationException($"Cannot get guid for {prefab.name}");
        }
#endif
    }
}