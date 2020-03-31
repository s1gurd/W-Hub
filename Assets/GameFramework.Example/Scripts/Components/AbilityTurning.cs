using System;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityTurning : MonoBehaviour, IActorAbility
    {
        [EnumToggleButtons] public ActorRotationMode rotationMode;

        [ShowIf("@rotationMode == ActorRotationMode.FollowMovement")]
        public float rotationSpeed = 1f;

        [ShowIf("@rotationMode == ActorRotationMode.LookAtMouse")]
        public float camRayLen = 20f;

        [ShowIf("@rotationMode == ActorRotationMode.LookAtMouse")]
        public string groundLayerName = "Ground";

        [ShowIf("@rotationMode == ActorRotationMode.RotateByLookInput")]
        public float sensitivity = 3f;


        [ShowIf("@rotationMode == ActorRotationMode.RotateByLookInput")]
        [Title("Allow rotation by axis", horizontalLine: false, bold: false)]
        [ToggleLeft]
        public bool Horizontal = false;

        [ShowIf("@rotationMode == ActorRotationMode.RotateByLookInput")] [ToggleLeft]
        public bool Vertical = false;

        [ShowIf("@rotationMode == ActorRotationMode.RotateByLookInput")] [ToggleLeft]
        public bool invertVertical = false;

        public void AddComponentData(ref Entity entity)
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            switch (rotationMode)
            {
                case ActorRotationMode.LookAtMouse:
                    dstManager.AddComponentData(entity, new ActorRotationLookAtMouseData
                    {
                        CamRayLen = camRayLen,
                        Layer = LayerMask.GetMask(groundLayerName)
                    });
                    break;
                case ActorRotationMode.FollowMovement:
                    dstManager.AddComponentData(entity, new ActorRotationFollowMovementData
                    {
                        RotationSpeed = rotationSpeed
                    });
                    break;
                case ActorRotationMode.RotateByLookInput:
                    dstManager.AddComponentData(entity, new ActorRotationByLookInputData
                    {
                        Sensitivity = sensitivity,
                        RotateX = Horizontal,
                        RotateY = Vertical,
                        Invert = invertVertical
                    });
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Execute()
        {
        }
    }

    public struct ActorRotationFollowMovementData : IComponentData
    {
        public float RotationSpeed;
    }

    public struct ActorRotationLookAtMouseData : IComponentData
    {
        public float CamRayLen;
        public int Layer;
    }

    public struct ActorRotationByLookInputData : IComponentData
    {
        public float Sensitivity;
        public bool RotateX;
        public bool RotateY;
        public bool Invert;
    }

    public enum ActorRotationMode
    {
        LookAtMouse = 0,
        FollowMovement = 1,
        RotateByLookInput = 2
    }
}