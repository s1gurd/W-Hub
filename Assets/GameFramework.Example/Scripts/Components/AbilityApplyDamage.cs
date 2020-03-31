using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components.Interfaces;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Components
{
    [HideMonoScript]
    public class AbilityApplyDamage : MonoBehaviour, IActorDataExchange
    {
        [SerializeField] private int damageValue;
        
        public IActor TargetActor { get; set; }

        public void AddComponentData(ref Entity entity)
        {
        }

        public void Execute()
        {
            if (TargetActor == null) return;

            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            if (!dstManager.HasComponent<AppliedDamageData>(TargetActor.ActorEntity))
            {
                dstManager.AddComponentData(TargetActor.ActorEntity, new AppliedDamageData
                {
                    DamageValue = damageValue
                });

                return;
            }

            var damageData = dstManager.GetComponentData<AppliedDamageData>(TargetActor.ActorEntity);
            damageData.DamageValue += damageValue;
            dstManager.SetComponentData(TargetActor.ActorEntity,damageData);
        }
    }

    public struct AppliedDamageData : IComponentData
    {
        public int DamageValue;
    }
}