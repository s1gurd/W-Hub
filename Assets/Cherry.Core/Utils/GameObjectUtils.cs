using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Utils
{
    public static class GameObjectUtils
    {
        public static void DestroyWithEntity(this GameObject go, Entity entity)
        {
            Object.Destroy(go);
            World.DefaultGameObjectInjectionWorld.EntityManager.DestroyEntity(entity);
        }

        public static T SafeAddComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }
    }
}