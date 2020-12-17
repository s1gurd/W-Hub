using System;
using System.Collections.Generic;
using System.Linq;

using GameFramework.Example.Loading.Interfaces;

using UnityEngine;

namespace GameFramework.Example.Loading.Repositories
{
    public sealed class GameObjectRepositoryFromSceneObject : MonoBehaviour, IGameObjectRepository
    {
        public GameObject[] gameObjects;

        T IGameObjectRepository.Get<T>(string name)
        {
            var foundGameObject = gameObjects.FirstOrDefault(prefab => string.Equals(prefab.name, name, StringComparison.Ordinal)) as T;

            if (foundGameObject == null)
            {
                throw new KeyNotFoundException($"GameObject with name '{name}' not found at GameObject repository!");
            }

            return foundGameObject;
        }
    }
}