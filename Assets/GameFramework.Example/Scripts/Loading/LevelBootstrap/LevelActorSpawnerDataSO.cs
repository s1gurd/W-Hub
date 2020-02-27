using GameFramework.Example.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameFramework.Example.Loading.LevelBootstrap
{
    [CreateAssetMenu(fileName = "Level Actor Spawner", menuName = "Game.Framework/Create Level Spawner Settings file", order = 1)]
    [HideMonoScript]
    public class LevelActorSpawnerDataSO:ScriptableObject
    {
        public ActorSpawnerSettings SpawnData;
    }
}