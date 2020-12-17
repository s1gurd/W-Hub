using GameFramework.Example.Utils;
using Unity.Entities;
using Unity.Mathematics;

namespace GameFramework.Example.Common
{
    public struct MoveByInputData : IComponentData
    {
    }
    
    public struct MoveDirectlyData : IComponentData
    {
        public float3 Position;
        public float Speed;
    }

    public struct RotateDirectlyData : IComponentData
    {
        public float3 Rotation;
        public bool3 Constraints;
    }

    public struct TimerData : IComponentData
    {
    }
    
    [NetworkSimObject]
    public struct DamageData : IComponentData
    {
        [NetworkSimData]
        public float DamageValue;
        [NetworkSimData]
        public Entity AbilityOwnerEntity;
        [NetworkSimData]
        public Entity TargetEntity;
    }
}