using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    /// <summary>
    /// System for hard syncing Actor Transform to GameState
    /// To turn on, remove DisableAutoCreation attribute
    /// </summary>
    [DisableAutoCreation]
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    [UpdateAfter(typeof(NetworkInputSystem))]
    public class InputCompensateCameraRotation : ComponentSystem
    {
        private EntityQuery _query;

        public NetworkSyncedData syncedData;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<Transform>(),
                ComponentType.ReadOnly<PlayerInputData>(),
                ComponentType.ReadOnly<NetworkInputData>());
        }


        protected override void OnUpdate()
        {
            Entities.With(_query).ForEach((Entity entity, Transform transform) =>
            {
                //Check if Position and Rotation are in sync and put them in place if not
                if (!CheckSync(transform))
                {
                    PostUpdateCommands.AddComponent(entity, new MoveDirectlyData
                    {
                        Position = SyncPosition()
                    });
                    PostUpdateCommands.AddComponent(entity, new RotateDirectlyData
                    {
                        Constraints = new bool3(true),
                        Rotation = SyncRotation()
                    });
                }
            });
        }

        private float3 SyncPosition()
        {
            return float3.zero;
        }

        private float3 SyncRotation()
        {
            return float3.zero;
        }

        private bool CheckSync(Transform t)
        {
            return true;
        }
    }

    public struct NetworkSyncedData
    {
        public Vector3 Position;
    }
}