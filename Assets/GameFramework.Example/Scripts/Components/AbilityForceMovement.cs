using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityForceMovement : MonoBehaviour, IActorAbility
    {
        [InfoBox("To set movement speed and dynamics, use AbilityMovement component")]
        [EnumToggleButtons] public MoveDirection moveDirection;

        [ShowIf("moveDirection", MoveDirection.UseDirection)]
        public Vector3 forwardVector;
        
        [ShowIf("moveDirection", MoveDirection.UseDirection)]
        public bool compensateSpawnerRotation = true;
        
        [HideInInspector] public Transform spawner;
        public void AddComponentData(ref Entity entity)
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
           
            dstManager.AddComponentData(entity,new MoveByInputData());
            dstManager.AddComponentData(entity, new ActorForceMovementData
            {
                MoveDirection = moveDirection,
                ForwardVector = forwardVector,
                CompensateSpawnerRotation = compensateSpawnerRotation,
                stopGuiding = false
            });

        }

        private void Start()
        {
            GetSpawner();
        }

        public void GetSpawner()
        {
            spawner = this.gameObject.GetComponent<IActor>().Spawner.transform;
        }

        public void Execute()
        {
        }
    }

    public struct ActorForceMovementData : IComponentData
    {
        public MoveDirection MoveDirection;
        public float3 ForwardVector;
        public bool CompensateSpawnerRotation;
        public bool stopGuiding;
    }

    public enum MoveDirection
    {
        SpawnerForward = 0,
        UseDirection = 1,
        GuidedBySpawner = 2
    }
}