using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityFollowMovement : MonoBehaviour, IActorAbility
    {
        [EnumToggleButtons] public TargetType followTarget;

        [ShowIf("followTarget", TargetType.ComponentName)]
        public string actorWithComponentName;

        [ShowIf("followTarget", TargetType.ChooseByTag)] [ValueDropdown("Tags")]
        public string targetTag;

        [HideIf("followTarget", TargetType.Spawner)] [EnumToggleButtons]
        public ChooseTargetStrategy strategy;

        [Space] [EnumToggleButtons] public FollowType followMovementType;

        [ShowIf("followMovementType", FollowType.Simple)]
        [InfoBox("Speed of 0 results in unconstrained speed")]
        [MinValue(0)]
        public float movementSpeed = 0;

        [ShowIf("followMovementType", FollowType.Simple)]
        public bool retainOffset = false;

        [HideInInspector] public Transform target;

        private static IEnumerable Tags()
        {
            return UnityEditorInternal.InternalEditorUtility.tags;
        }

        public void AddComponentData(ref Entity entity)
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            dstManager.AddComponentData(entity, new ActorFollowMovementData());

            dstManager.AddComponentData(entity, new ActorNoFollowTargetMovementData());

            if (followMovementType == FollowType.UseMovementComponent)
            {
                dstManager.AddComponentData(entity, new MoveByInputData());
            }
            else
            {
                dstManager.AddComponentData(entity, new MoveDirectlyData
                {
                    Speed = movementSpeed
                });
            }
        }


        public void Execute()
        {
        }
    }

    public struct ActorFollowMovementData : IComponentData
    {
        public float3 Origin;
    }

    public struct ActorNoFollowTargetMovementData : IComponentData
    {
    }
}