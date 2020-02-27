using System.Collections.Generic;
using GameFramework.Example.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Loading.LevelBootstrap
{
    [HideMonoScript]
    public class LevelGameBootstrap : MonoBehaviour, IGameModeBootstrap
    {
        private readonly List<IActorSpawner> _actorSpawners = new List<IActorSpawner>();

        private void Start()
        {
            Application.targetFrameRate = 60;
            CollectSpawners();
            RunSpawners();
            RunSpawnActions();
        }

        public void CollectSpawners(List<IActorSpawner> spawners)
        {
            foreach (var spawner in GetComponents<IActorSpawner>())
            {
                spawners.Add(spawner);
            }
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
    }
}