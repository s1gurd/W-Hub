using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityUIActorFollowSpawner : MonoBehaviour, IActorAbility
    {
        public RectTransform RelatedCanvasRect = null;
        public Vector3 Offset = new Vector3();
        public IActor Actor { get; set; }

        public void AddComponentData(ref Entity entity, IActor actor)
        {
            Actor = actor;
            
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            dstManager.AddComponentData(entity, new UIActorFollowMovementData());
            
            dstManager.AddComponentData(entity, new MoveDirectlyData());
        }

        public void Execute()
        {
        }
    }
    
    public struct UIActorFollowMovementData : IComponentData
    {
    }
}