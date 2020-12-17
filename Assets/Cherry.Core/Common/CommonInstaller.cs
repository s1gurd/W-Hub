using System.Threading.Tasks;
using Cherry.Core.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Utils;
using SimpleContainer;
using SimpleContainer.Unity.Installers;
using Sirenix.OdinInspector;

namespace GameFramework.Example.Common
{
    [HideMonoScript]
    public class CommonInstaller : MonoInstaller
    {
        public override void Install(Container container)
        {
            container.RegisterAttribute<InjectAttribute>();
            
            container.Register<IGameSettings, GameSettings>(Scope.Singleton);
        }

        public override Task ResolveAsync(Container container)
        {
            container.Resolve<IGameSettings>();
            return Task.CompletedTask;
        }
    }
}