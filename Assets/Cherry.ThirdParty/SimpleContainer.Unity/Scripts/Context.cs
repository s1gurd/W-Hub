using System;

using JetBrains.Annotations;

using UnityEngine;

namespace SimpleContainer.Unity
{
    // !!! [Obsolete("Do not use a service locator! Inject dependencies instead.")] !!!
    public static class Context
    {
        public static event Action ContainerAwaken;

        private static bool _appQuitting  = false;
        private static ulong _usageCount = 0;

        public static UnityProjectRoot ProjectRoot
        {
            set;
            private get;
        }
        
        public static UnitySceneRoot SceneRoot
        {
            set;
            private get;
        }
        
        public static Container CurrentContainer
        {
            get
            {
                var result = FindContainer();

                if (result == null)
                    Debug.LogError("No DI containers found in current context!");

                return result;
            }
        }

        public static UnityProjectRoot GetProjectRoot()
        {
            return ProjectRoot;
        }

        public static UnitySceneRoot GetSceneRoot()
        {
            return SceneRoot;
        }

        public static bool CheckHasContainer()
        {
            var container = FindContainer();
            return container != null;
        }

        [CanBeNull]
        public static Container FindContainer()
        {
            _usageCount++;

            if (_appQuitting)
                return null;

            if (ProjectRoot != null)
                return ProjectRoot.Container;

            if (SceneRoot != null)
                return SceneRoot.Container;

            return null;
        }

        public static void InvokeContainerAwaken()
        {
            ContainerAwaken?.Invoke();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void RunOnStart()
        {
            _usageCount = 0;

            Application.quitting += () =>
            {
                _appQuitting = true;
                if (_usageCount > 0) Debug.Log($"[SIMPLE CONTAINER] We called Service Locator {_usageCount} times!!! What a shame!!!");
            };
        }
    }
}