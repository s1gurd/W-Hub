using GameFramework.Example.Common;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityForceRotation : MonoBehaviour, IActorAbility
    {
        public Vector3 rotationDelta;
        public float rotationSpeed;
        public void AddComponentData(ref Entity entity)
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            dstManager.AddComponentData(entity, new ActorForceRotationData
            {
                RotationDelta = rotationDelta,
                RotationSpeed = rotationSpeed
            });

            dstManager.AddComponentData(entity, new RotateDirectlyData
            {
                Constraints = new bool3(!rotationDelta.x.Equals(0),!rotationDelta.y.Equals(0),!rotationDelta.z.Equals(0))
            });
        }
        

        public void Execute()
        {
        }
    }

    public struct ActorForceRotationData : IComponentData
    {
        public Vector3 RotationDelta;
        public float RotationSpeed;
    }
}