using System;
using System.Collections.Generic;
using GameFramework.Example.Common;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityPlayerInput : MonoBehaviour, IActorAbility
    {
        [EnumToggleButtons] public InputSource inputSource;

        public bool compensateCameraRotation = true;

        [Space]
        [InfoBox(
            "Bind Abilities calls to Custom Inputs, indexes 0..9 represent keyboard keys of 0..9.\n" +
            "Further bindings are as set in User Input")]
        public List<CustomBinding> customBindings = new List<CustomBinding>();
        
        

        public void AddComponentData(ref Entity entity)
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            dstManager.AddComponentData(entity, new PlayerInputData());
            dstManager.AddComponentData(entity, new MoveByInputData());
            if (compensateCameraRotation)
            {
                dstManager.AddComponentData(entity, new CompensateCameraRotation());
            }
            
            switch (inputSource)
            {
                case InputSource.Default:
                    break;
                case InputSource.UserInput:
                    dstManager.AddComponentData(entity, new UserInputData());
                    break;
                case InputSource.NetworkInput:
                    dstManager.AddComponentData(entity, new NetworkInputData());
                    break;
                case InputSource.AIInput:
                    dstManager.AddComponentData(entity, new AIInputData());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Execute()
        {
        }
    }

    public struct PlayerInputData : IComponentData
    {
        public float2 Move;
        public float2 Mouse;
        public float2 Look;
        public float CompensateAngle;
        public FixedList512<float> CustomInput;
    }

    public struct UserInputData : IComponentData
    {
        public byte foo;
    }

    public struct NetworkInputData : IComponentData
    {
        public byte foo;
    }

    public struct AIInputData : IComponentData
    {
        public byte foo;
    }

    public struct CompensateCameraRotation : IComponentData
    {
    }
}