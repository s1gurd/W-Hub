using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
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
        public IActor Actor { get; set; }

        [EnumToggleButtons] public InputSource inputSource;

        [ShowIfGroup("inputSource", InputSource.UserInput)]
        public bool compensateCameraRotation = true;

        [ShowIfGroup("inputSource", InputSource.UserInput)]
        public float minMoveInputMagnitude = 0f;

        [ShowIf("inputSource", InputSource.AIInput)]
        [InfoBox("Additional AI Ability Component is needed to use AI Input")]
        [ValidateInput("MustBeAI", "AI MonoBehaviour must exist and derive from IAISettings!")]
        [LabelText("AI Behaviour Component")]
        public MonoBehaviour AIBehaviour;

        [Space]
        [InfoBox(
            "Bind Abilities calls to Custom Inputs, indexes 0..9 represent keyboard keys of 0..9.\n" +
            "Further bindings are as set in User Input")]
        public List<CustomBinding> customBindings = new List<CustomBinding>();

        [HideInInspector] public Dictionary<int, List<IActorAbility>> bindingsDict;

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            bindingsDict = new Dictionary<int, List<IActorAbility>>();

            Actor = actor;

            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var c = new FixedList512<float> {Length = Constants.INPUT_BUFFER_CAPACITY};
            var sticksInput = new FixedList512<float2> {Length = Constants.INPUT_BUFFER_CAPACITY};

            dstManager.AddComponentData(entity, new PlayerInputData
            {
                CustomInput = c,
                MinMagnitude = minMoveInputMagnitude,
                CustomSticksInput = sticksInput
            });

            foreach (var binding in customBindings)
            {
                AddCustomBinding(binding);
            }

            dstManager.AddComponentData(entity, new MoveByInputData());

            switch (inputSource)
            {
                case InputSource.Default:
                    break;
                case InputSource.UserInput:
                    dstManager.AddComponentData(entity, new UserInputData());
                    dstManager.AddComponentData(entity, new NetworkSyncSend());
                    if (compensateCameraRotation)
                    {
                        dstManager.AddComponentData(entity, new CompensateCameraRotation());
                    }

                    break;
                case InputSource.NetworkInput:
                    dstManager.AddComponentData(entity, new NetworkInputData());
                    break;
                case InputSource.AIInput:
                    dstManager.AddComponentData(entity, new AIInputData());
                    dstManager.AddComponentData(entity, new NetworkSyncSend());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Execute()
        {
        }

        public void AddCustomBinding(CustomBinding binding)
        {
            if (!customBindings.Contains(binding)) customBindings.Add(binding);

            if (!bindingsDict.Keys.Contains(binding.index))
            {
                bindingsDict.Add(binding.index, binding.actions.ConvertAll(a => a as IActorAbility));
            }
            else
            {
                bindingsDict[binding.index].AddRange(binding.actions.ConvertAll(a => a as IActorAbility));
            }
        }

        public void RemoveCustomBinding(int indexToRemove)
        {
            var bindingToRemove = customBindings.FirstOrDefault(b => b.index == indexToRemove);
            customBindings.Remove(bindingToRemove);

            bindingsDict.Remove(indexToRemove);
        }

        private bool MustBeAI(MonoBehaviour a)
        {
            return a is IAIModule;
        }
    }

    [NetworkSimObject]
    public struct PlayerInputData : IComponentData
    {
        [NetworkSimData] public float2 Move;
        [NetworkSimData] public float2 Mouse;
        [NetworkSimData] public float2 Look;
        [NetworkSimData] public float CompensateAngle;
        public float MinMagnitude;
        [NetworkSimData] public FixedList512<float> CustomInput;
        [NetworkSimData] public FixedList512<float2> CustomSticksInput;
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