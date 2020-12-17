using System.Collections.Generic;
using GameFramework.Example.Components;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    public class AISetupSystem : ComponentSystem
    {
        private EntityQuery _queryAI;
        private EntityQuery _queryTargets;

        private List<Transform> _targets;

        protected override void OnCreate()
        {
            _queryAI = GetEntityQuery(
                ComponentType.ReadWrite<SetupAIData>(),
                ComponentType.ReadOnly<AIInputData>(),
                ComponentType.ReadOnly<AbilityAIInput>(),
                ComponentType.ReadOnly<Transform>(),
                ComponentType.Exclude<DeadActorData>(),
                ComponentType.Exclude<DestructionPendingData>(),
                ComponentType.Exclude<EvaluateAIData>(),
                ComponentType.Exclude<NetworkInputData>());
        }

        protected override void OnUpdate()
        {
            _targets = new List<Transform>();

            Entities.With(_queryAI).ForEach(
                (Entity entity, Transform transform, AbilityAIInput ai) =>
                {
                    
                    
                    if (ai.activeBehaviour == null)
                    {
                        Debug.LogError(
                            "[SETUP AI SYSTEM] AI Behaviour marked for AI Setup, but activeBehaviour is null! Aborting AI for this Actor: " +
                            transform.gameObject);
                        World.EntityManager.RemoveComponent<AIInputData>(entity);
                        return;
                    }

                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (ai.activeBehaviourPriority == 0) return;

                    if (!ai.activeBehaviour.BehaviourInstance.SetUp(entity, World.EntityManager))
                    {
                        ai.EvaluateAll();
                    }

                    World.EntityManager.RemoveComponent<SetupAIData>(entity);
                }
            );
        }
    }
}