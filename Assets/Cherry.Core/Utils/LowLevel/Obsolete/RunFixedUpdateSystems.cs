using System;
using GameFramework.Example.Systems;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Utils.LowLevel
{
    public static class RunFixedUpdateSystems
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void SetFixedUpdate()
        {
            FixedRateUtils.EnableFixedRateSimple(World.DefaultGameObjectInjectionWorld.GetExistingSystem<FixedUpdateGroup>(), UnityEngine.Time.fixedDeltaTime);
        }
        
    }
}