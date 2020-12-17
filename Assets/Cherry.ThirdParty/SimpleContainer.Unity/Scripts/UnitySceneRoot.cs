using SimpleContainer.Unity.Installers;

using UnityEngine;

namespace SimpleContainer.Unity
{
    public class UnitySceneRoot : MonoBehaviour
    {
        public MonoInstaller[] installers;
        public bool enableResolveCheck = true;

        public Container Container { get; private set; }

        void Awake()
        {
            Context.InvokeContainerAwaken();

            if (Context.GetProjectRoot() == null)
            {
                var projectRootContainer = Container.Create();

                foreach (MonoInstaller installer in installers)
                    projectRootContainer.Install(installer);

                Container = projectRootContainer;
            }
            else
            {
                var sceneContainer = Container.Create();

                foreach (MonoInstaller installer in installers)
                    sceneContainer.Install(installer);

                var projectRootContainer = Context.GetProjectRoot().Container;

                projectRootContainer.OverrideFrom(sceneContainer);

                Container = projectRootContainer;
            }

            Context.SceneRoot = this;
        }

        async void Start()
        {
            Container.InjectIntoRegistered();

            foreach (MonoInstaller installer in installers)
                await installer.ResolveAsync(Container);

            if (enableResolveCheck)
                Container.ThrowIfNotResolved();
        }

        void OnDestroy()
        {
            Context.SceneRoot = null;
        }
    }
}