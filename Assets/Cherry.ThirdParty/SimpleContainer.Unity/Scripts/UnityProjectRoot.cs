using System.Threading.Tasks;

using SimpleContainer.Unity.Installers;

using UnityEngine;

namespace SimpleContainer.Unity
{
    public class UnityProjectRoot : MonoBehaviour
    {
        public MonoInstaller[] installers;
        public bool InstallManually;
        public bool enableResolveCheck = true;

        public Container Container { get; private set; }

        void Awake()
        {
            if (InstallManually)
                return;

            Install();
        }

        async void Start()
        {
            if (InstallManually)
                return;

            await ResolveAsync();
        }

        void OnDestroy()
        {
            Context.ProjectRoot = null;
        }

        public async Task InstallAsync()
        {
            Install();
            await ResolveAsync();
        }

        private void Install()
        {
            Context.InvokeContainerAwaken();

            Container = Container.Create();

            foreach (MonoInstaller installer in installers)
                Container.Install(installer);

            Context.ProjectRoot = this;
        }

        private async Task ResolveAsync()
        {
            Container.InjectIntoRegistered();

            foreach (MonoInstaller installer in installers)
                await installer.ResolveAsync(Container);

            if (enableResolveCheck)
                Container.ThrowIfNotResolved();
        }
    }
}