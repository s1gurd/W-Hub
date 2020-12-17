using System.Collections.Generic;
using GameFramework.Example.Components;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.AI
{
    public interface IAIBehaviour
    {
        string XAxis { get; }
        string[] AdditionalModes { get; }
        bool NeedCurve { get; }
        bool NeedTarget { get; }
        bool NeedActions { get; }

        float Evaluate(Entity entity, AIBehaviourSetting behaviourSetting, AbilityAIInput ai, List<Transform> targets);
        bool SetUp(Entity entity, EntityManager dstManager);
        bool Behave(Entity entity, EntityManager dstManager, ref PlayerInputData inputData);
    }
}