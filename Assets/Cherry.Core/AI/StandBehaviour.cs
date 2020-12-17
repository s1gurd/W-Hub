using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GameFramework.Example.AI
{
    [Serializable]
    public class StandBehaviour : IAIBehaviour
    {
        public string XAxis => "";

        public string[] AdditionalModes => new string[0];

        public bool NeedCurve => false;
        public bool NeedTarget => false;
        public bool NeedActions => false;

        private const float FINISH_ROAM_DISTSQ = 2f;
        private const float PRIORITY_MULTIPLIER = 0.5f;

        private AIBehaviourSetting _behaviour = null;
        private Transform _transform = null;
        private readonly NavMeshPath _path = new NavMeshPath();

        private int _currentWaypoint = 0;

        public float Evaluate(Entity entity, AIBehaviourSetting behaviour, AbilityAIInput ai, List<Transform> targets)
        {
            _behaviour = behaviour;
            _transform = _behaviour.Actor.GameObject.transform;

            return Random.value * _behaviour.basePriority;
        }

        public bool SetUp(Entity entity, EntityManager dstManager)
        {
            return true;
        }

        public bool Behave(Entity entity, EntityManager dstManager, ref PlayerInputData inputData)
        {
            

            return true;
        }
    }
}