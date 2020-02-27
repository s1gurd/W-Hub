using System;
using Unity.Entities;
using UnityEngine;



namespace GameFramework.Example.Utils.LowLevel
{
    public class RunFixedUpdateSystems
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void MoveSimulationGroup()
        {
            // This must be called AFTER DefaultWorldInitialization, otherwise DefaultWorldInitialization overwrites PlayerLoop
            var playerLoop = ScriptBehaviourUpdateOrder.CurrentPlayerLoop;
            var func = RemoveCallback<SimulationSystemGroup>(playerLoop);
            
            if (func == null) return;
            
            InstallCallback<SimulationSystemGroup>(playerLoop, typeof(UnityEngine.PlayerLoop.FixedUpdate), func);
            ScriptBehaviourUpdateOrder.SetPlayerLoop(playerLoop);
        }

        private static void InstallCallback<T>(UnityEngine.LowLevel.PlayerLoopSystem playerLoop, Type subsystem,
            UnityEngine.LowLevel.PlayerLoopSystem.UpdateFunction callback)
        {
            for (var i = 0; i < playerLoop.subSystemList.Length; ++i)
            {
                int subsystemListLength = playerLoop.subSystemList[i].subSystemList.Length;
                
                if (playerLoop.subSystemList[i].type != subsystem) continue;
                
                // Create new subsystem list and add callback
                var newSubsystemList = new UnityEngine.LowLevel.PlayerLoopSystem[subsystemListLength + 1];
                for (var j = 0; j < subsystemListLength; j++)
                {
                    newSubsystemList[j] = playerLoop.subSystemList[i].subSystemList[j];
                }

                newSubsystemList[subsystemListLength].type = typeof(UnityEngine.PlayerLoop.FixedUpdate);
                newSubsystemList[subsystemListLength].updateDelegate = callback;
                playerLoop.subSystemList[i].subSystemList = newSubsystemList;
            }
        }

        static UnityEngine.LowLevel.PlayerLoopSystem.UpdateFunction RemoveCallback<T>(UnityEngine.LowLevel.PlayerLoopSystem playerLoop)
        {
            for (var i = 0; i < playerLoop.subSystemList.Length; ++i)
            {
                var subsystemListLength = playerLoop.subSystemList[i].subSystemList.Length;
                for (var j = 0; j < subsystemListLength; j++)
                {
                    var item = playerLoop.subSystemList[i].subSystemList[j];
                    if (item.type != typeof(T)) continue;
                    playerLoop.subSystemList[i].subSystemList =
                        ExceptIndex(playerLoop.subSystemList[i].subSystemList, j);
                    return item.updateDelegate;
                }
            }

            return null;
        }

        static T[] ExceptIndex<T>(T[] array, int exceptIndex)
        {
            var result = new T[array.Length - 1];
            if (exceptIndex > 0)
            {
                Array.Copy(array, result, exceptIndex);
            }

            if (exceptIndex < array.Length - 1)
            {
                Array.Copy(array, exceptIndex + 1, result, exceptIndex, array.Length - exceptIndex - 1);
            }

            return result;
        }
    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class FixedUpdateGroup : ComponentSystemGroup
    {
    }
}