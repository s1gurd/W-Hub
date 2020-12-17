using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using Unity.Mathematics;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class ActorForceRotationSystem : ComponentSystem
    {
        private EntityQuery _query;
        
        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<RotateDirectlyData>(),
                ComponentType.ReadOnly<ActorForceRotationData>(),
                ComponentType.Exclude<StopRotationData>());
        }

        protected override void OnUpdate()
        {
            var dt = Time.DeltaTime;
            
            Entities.With(_query).ForEach(
                (Entity entity, ref RotateDirectlyData rotation, ref ActorForceRotationData forceRotation) =>
                {
                    rotation.Rotation += (float3)forceRotation.RotationDelta * (forceRotation.RotationSpeed * dt);
                });
        }
    }
}