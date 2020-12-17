using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Loading.ActorSpawners;
using GameFramework.Example.Loading.Interfaces;

namespace GameFramework.Example.Loading
{
    public sealed class ActorSpawnerFromMetaFactory : IActorSpawnerFromMetaFactory
    {
        /*
        private readonly IPrefabRepository _prefabRepository;
        private readonly IGameObjectRepository _gameObjectRepository;
        private readonly IConfigService _configService;

        public ActorSpawnerFromMetaFactory(
            IPrefabRepository       prefabRepository,
            IGameObjectRepository   gameObjectRepository,
            IConfigService          configService)
        {
            _prefabRepository = prefabRepository;
            _gameObjectRepository = gameObjectRepository;
            _configService = configService;
        }

        IActorSpawner IActorSpawnerFromMetaFactory.Create(MatchPlayer matchPlayer)
        {
            return new LevelActorFromMeta(_prefabRepository, _gameObjectRepository, _configService, matchPlayer);
        }

        IActorSpawner IActorSpawnerFromMetaFactory.CreateDummy(MatchPlayer matchPlayer, IConfigService configService)
        {
            return new LevelActorFromMeta(_prefabRepository, _gameObjectRepository, configService, matchPlayer);
        }*/
    }
}