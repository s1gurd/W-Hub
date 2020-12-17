using GameFramework.Example.Common;
using GameFramework.Example.Components;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    public class ActorApplyDamageSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            Entities.WithAll<DamageData>().ForEach((Entity damageEntity, ref DamageData damageData) =>
            {
                if (!(dstManager.Exists(damageData.TargetEntity) &&
                      dstManager.Exists(damageData.AbilityOwnerEntity))) return;

                var abilityOwner = dstManager.GetComponentObject<AbilityActorPlayer>(damageData.AbilityOwnerEntity);

                if (dstManager.HasComponent<AbilityActorPlayer>(damageData.TargetEntity) &&
                    !dstManager.HasComponent<DeadActorData>(damageData.TargetEntity) &&
                    !dstManager.HasComponent<DestructionPendingData>(damageData.TargetEntity))
                {
                    var target = dstManager.GetComponentObject<AbilityActorPlayer>(damageData.TargetEntity);

                    if (target.IsAlive)
                    {
                        target.UpdateHealthData(-damageData.DamageValue);
                        abilityOwner.UpdateTotalDamageData(damageData.DamageValue);

                        if (dstManager.HasComponent<UserInputData>(abilityOwner.Actor.ActorEntity) ||
                            dstManager.HasComponent<UserInputData>(target.Actor.ActorEntity))
                        {
                            target.ShowReceivedDamageNumber(damageData.DamageValue);
                        }
                    }

                    if (!target.IsAlive)
                    {
                        abilityOwner.UpdateExperienceData(GameMeta.PointsForKill);
                    }
                }

                if (dstManager.HasComponent<AbilityDestructibleObject>(damageData.TargetEntity))
                {
                    var target = dstManager.GetComponentObject<AbilityDestructibleObject>(damageData.TargetEntity);

                    if (target != null)
                    {
                        target.UpdateStrengthValue((int) -damageData.DamageValue);
                        abilityOwner.UpdateTotalDamageData((int) damageData.DamageValue);
                    }
                }

                PostUpdateCommands.DestroyEntity(damageEntity);
            });
        }
    }
}