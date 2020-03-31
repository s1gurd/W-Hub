using System.Collections;
using GameFramework.Example.Common;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityFollowRotation : MonoBehaviour, IActorAbility
    {
        [EnumToggleButtons] public TargetType followTarget;

        [ShowIf("followTarget", TargetType.ComponentName)]
        public string actorWithComponentName;

        [ShowIf("followTarget", TargetType.ChooseByTag)] [ValueDropdown("Tags")]
        public string targetTag;

        [HideIf("followTarget", TargetType.Spawner)] [EnumToggleButtons]
        public ChooseTargetStrategy strategy;

        public bool followX = false;
        public bool followY = true;
        public bool followZ = false;

        public bool retainRotationOffset = true;
        
        [HideInInspector] public Transform target;
        
        private static IEnumerable Tags()
        {
            return EditorUtils.GetEditorTags();
        }
        public void AddComponentData(ref Entity entity)
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            dstManager.AddComponentData(entity, new ActorFollowRotationData());
            dstManager.AddComponentData(entity, new ActorNoFollowTargetRotationData());

            dstManager.AddComponentData(entity, new RotateDirectlyData
            {
                Constraints = new bool3(followX,followY,followZ)
            });
        }


        public void Execute()
        {
        }
    }

    public struct ActorFollowRotationData : IComponentData
    {
        public float3 Origin;
    }

    public struct ActorNoFollowTargetRotationData : IComponentData
    {
    }
}