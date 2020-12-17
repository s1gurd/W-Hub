using System;
using GameFramework.Example.Common.Interfaces;
using SimpleContainer.Unity;

namespace Cherry.Core
{
    public class Resolver
    {
        private readonly Lazy<IGameSettings> _gameSettings = new Lazy<IGameSettings>(() => Context.CurrentContainer.Resolve<IGameSettings>());

        public bool HasContainer => Context.CheckHasContainer();
        
        public IGameSettings GameSettings => _gameSettings.Value;
    }
}