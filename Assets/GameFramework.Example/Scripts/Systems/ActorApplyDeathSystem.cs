using GameFramework.Example.Components;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    public class ActorApplyDeathSystem : ComponentSystem
    {
        private EntityQuery _destructionActorByTimerQuery;
        private EntityQuery _immediateActorDestructionQuery;

        protected override void OnCreate()
        {
            _destructionActorByTimerQuery = GetEntityQuery(ComponentType.ReadOnly<DeadActorData>(),
                ComponentType.Exclude<ImmediateActorDestructionData>(),
                ComponentType.ReadOnly<Transform>());

            _immediateActorDestructionQuery = GetEntityQuery(ComponentType.ReadOnly<ImmediateActorDestructionData>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_destructionActorByTimerQuery).ForEach(
                (Entity entity, Transform obj) =>
                {
                    var go = obj.gameObject;
                    var health = go.GetComponent<AbilityActorHealth>();

                    if (health == null || !health.TimerActive) return;
                    
                    health.StartDeathTimer();
                }
            );

            Entities.With(_immediateActorDestructionQuery).ForEach(
                (Entity entity, Transform obj) =>
                {
                    Object.Destroy(obj.gameObject);
                    PostUpdateCommands.DestroyEntity(entity);
                });
        }
    }
}