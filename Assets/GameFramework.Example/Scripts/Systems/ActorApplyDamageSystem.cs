using GameFramework.Example.Components;
using Unity.Entities;

namespace GameFramework.Example.Systems
{
    public class ActorApplyDamageSystem : ComponentSystem
    {
        private EntityQuery _entitiesToApplyDamageQuery;
        private EntityQuery _noHealthQuery;

        protected override void OnCreate()
        {
            _entitiesToApplyDamageQuery = GetEntityQuery(
                ComponentType.ReadOnly<ActorHealthData>(),
                ComponentType.ReadWrite<AppliedDamageData>(),
                ComponentType.ReadOnly<AbilityActorHealth>());

            _noHealthQuery = GetEntityQuery(
                ComponentType.Exclude<ActorHealthData>(),
                ComponentType.ReadWrite<AppliedDamageData>());
        }


        protected override void OnUpdate()
        {
            var dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            Entities.With(_entitiesToApplyDamageQuery).ForEach(
                (Entity entity, AbilityActorHealth actorHealth, ref ActorHealthData healthData,
                    ref AppliedDamageData damageData) =>
                {
                    dstManager.RemoveComponent<AppliedDamageData>(entity);

                    if (dstManager.HasComponent<DeadActorData>(entity)) return;

                    healthData.HealthValue -= damageData.DamageValue;
                    actorHealth.health = healthData.HealthValue;

                    if (healthData.HealthValue <= 0)
                    {
                        dstManager.AddComponent<DeadActorData>(entity);
                        return;
                    }
                    
                    dstManager.AddComponent<DamagedActorData>(entity);
                });

            Entities.With(_noHealthQuery)
                .ForEach(entity => dstManager.RemoveComponent<AppliedDamageData>(entity));
        }
    }
}