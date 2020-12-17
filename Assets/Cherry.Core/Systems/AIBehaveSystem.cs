using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class AIBehaveSystem : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<AIInputData>(),
                ComponentType.ReadOnly<PlayerInputData>(),
                ComponentType.ReadOnly<AbilityAIInput>(),
                ComponentType.ReadOnly<Transform>(),
                ComponentType.Exclude<DeadActorData>(),
                ComponentType.Exclude<EvaluateAIData>(),
                ComponentType.Exclude<SetupAIData>(),
                ComponentType.Exclude<DestructionPendingData>(),
                ComponentType.Exclude<NetworkInputData>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_query).ForEach(
                (Entity entity, AbilityAIInput ai, ref PlayerInputData input) =>
                {
                    if (ai.activeBehaviour == null)
                    {
                        Debug.LogError(
                            "[Behave AI SYSTEM] AI Behaviour marked for AI Setup, but activeBehaviour is null! Aborting AI for this Actor: " +
                            ai.transform.gameObject);
                        World.EntityManager.RemoveComponent<AIInputData>(entity);
                        return;
                    }
                    
                    if (!ai.activeBehaviour.BehaviourInstance.Behave(entity, World.EntityManager, ref input))
                    {
                        ai.EvaluateAll();
                        input.Move = float2.zero;
                        input.Look = float2.zero;
                        input.Mouse = float2.zero;
                        input.CustomInput = new FixedList512<float>{Length = Constants.INPUT_BUFFER_CAPACITY};
                        
                    }
                });
        }
    }
}