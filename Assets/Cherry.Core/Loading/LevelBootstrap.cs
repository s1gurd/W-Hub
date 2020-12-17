using System.Collections.Generic;

using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Loading.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;


namespace GameFramework.Example.Loading
{
    public class LevelBootstrap : MonoBehaviour, IGameModeBootstrap
    {
        //public SceneContext sceneContext;
        public bool useDummyMeta;

        private readonly List<IActorSpawner> _actorSpawners = new List<IActorSpawner>();

        //[Inject] public MatchParameters MatchParametersProvider { get; set; }

        //[Inject] public IActorSpawnerFromMetaFactory ActorSpawnerFromMetaFactory { get; set; }

        public void Start()
        {
            //sceneContext.Run();
            
           

            CollectSpawners();
            RunSpawners();
            RunSpawnActions();
        }

        public void CollectSpawners(List<IActorSpawner> spawners)
        {
            // Try to retrieve match parameters from meta loader:
            //var parameters = MatchParametersProvider;

            IEnumerable<IActorSpawner> actorSpawners;

            //if (parameters == null)
            {
                // Use spawners configured at scene:
                actorSpawners = GetComponents<IActorSpawner>();
            }
            //else
            //{
                // Use match parameters from meta loader:
            //    actorSpawners = GetActorSpawners(parameters);
            //}

            spawners.AddRange(actorSpawners);
        }

        public void RunSpawners(List<IActorSpawner> spawners)
        {
            foreach (var spawner in spawners)
            {
                spawner.Spawn();
            }
        }

        public void RunSpawnActions(List<IActorSpawner> spawners)
        {
            foreach (var spawner in spawners)
            {
                spawner.RunSpawnActions();
            }
        }

        public void CollectSpawners()
        {
            CollectSpawners(_actorSpawners);
        }

        public void RunSpawners()
        {
            RunSpawners(_actorSpawners);
        }

        public void RunSpawnActions()
        {
            RunSpawnActions(_actorSpawners);
        }
/*
        private IEnumerable<IActorSpawner> GetActorSpawners(MatchParameters parameters)
        {
            foreach (var matchPlayer in parameters.Players)
            {
                if (parameters.MatchType == MatchType.Local && matchPlayer.PlayerType == PlayerType.Network)
                    matchPlayer.PlayerType = PlayerType.Local;
                yield return ActorSpawnerFromMetaFactory.Create(matchPlayer);
            }
        }*/
    }
}