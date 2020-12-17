using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Components;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace GameFramework.Example.AI
{
    [Serializable]
    public class AttackBehaviour : IAIBehaviour
    {
        public string XAxis => "distance to closest seen target";
        public string[] AdditionalModes => new string[0];
        public bool NeedCurve => true;
        public bool NeedTarget => true;
        public bool NeedActions => true;

        private const float AIM_MAX_DIST = 40f;
        private const float SHOOTING_ANGLE_THRESH = 10f;
        private const float ATTACK_DELAY = 6f;
        private readonly Vector3 VIEW_POINT_DELTA = new Vector3(0f, 1f, 0f);

        private Transform _target = null;
        private Transform _transform = null;
        private AIBehaviourSetting _behaviour = null;
        private List<IActorAbility> _abilities = new List<IActorAbility>();
        private AbilityPlayerInput _input;

        private AbilityPlayerInput CustomInput
        {
            get
            {
                if (_input == null)
                {
                    _input = _behaviour.Actor.GameObject.GetComponent<AbilityPlayerInput>();
                }

                return _input;
            }
        }

        public float Evaluate(Entity entity, AIBehaviourSetting behaviour, AbilityAIInput ai,  List<Transform> targets)
        {
            if ( this.GetType().ToString().Contains(ai.activeBehaviour.behaviourType))
            {
                return 0f;
            }
            
            _target = null;
            _behaviour = behaviour;
            _transform = behaviour.Actor.GameObject.transform;

            if (Time.timeSinceLevelLoad < ATTACK_DELAY * Random.value) return 0f;

            if (World.DefaultGameObjectInjectionWorld.EntityManager.HasComponent<DeadActorData>(entity)) return 0f;
            
            if (CustomInput == null)
            {
                //Debug.Log("[ATTACK BEHAVIOUR] No Input Ability!");
                return 0f;
            }

            if (CustomInput.bindingsDict.ContainsKey(_behaviour.executeCustomInput))
            {
                _abilities = CustomInput.bindingsDict[_behaviour.executeCustomInput];
            }
            else
            {
                //Debug.Log($"[ATTACK BEHAVIOUR] Custom Input {_behaviour.executeCustomInput} in Attack Behaviour not set in Input Ability!");
                return 0f;
            }

            if (!_abilities.ActionPossible())
            {
                //Debug.Log($"[ATTACK BEHAVIOUR] Custom Input {_behaviour.executeCustomInput} has no actions!");
                return 0f;
            }

            List<Transform> orderedTargets = targets.Where(t => t.FilterTag(_behaviour) && t != _transform).OrderBy(t =>
                math.distancesq(_transform.position, t.position)).ToList();
            
            if (orderedTargets.Count == 0) return 0f;

            foreach (var t in orderedTargets)
            {
                //Debug.DrawRay(_transform.position + VIEW_POINT_DELTA, t.position - _transform.position, Color.blue, 2f);
                
                if (Physics.Raycast(_transform.position+VIEW_POINT_DELTA, t.position - _transform.position, out var hit,
                    AIM_MAX_DIST))
                {
                    if (hit.transform != t) continue;
                    _target = t;

                    var dist = math.distance(_transform.position, _target.position);
                    var sampleScale = _behaviour.curveMaxSample - _behaviour.curveMinSample;
                    var curveSample = math.clamp(
                        (dist - _behaviour.curveMinSample) / sampleScale, 0f, 1f);
                    var result = _behaviour.priorityCurve.Evaluate(curveSample) * _behaviour.basePriority;
                
                    return result;
                }
            }
            
            return 0f;
        }

        public bool SetUp(Entity entity, EntityManager dstManager)
        {
            return true;
        }

        public bool Behave(Entity entity, EntityManager dstManager, ref PlayerInputData inputData)
        {
            
            if (!_abilities.ActionPossible()) return false;

            if (Physics.Raycast(_transform.position + VIEW_POINT_DELTA, _target.position - _transform.position, out var hit,
                AIM_MAX_DIST) && hit.transform == _target)
            {
                var dir = _target.position - _transform.position;
                var angle = Vector2.Angle(new Vector2(dir.x, dir.z),
                    new Vector2(_transform.forward.x, _transform.forward.z));
                
                if (angle < SHOOTING_ANGLE_THRESH)
                {
                    inputData.CustomInput[_behaviour.executeCustomInput] = 1f;
                }
                else
                {
                    inputData.CustomInput[_behaviour.executeCustomInput] = 0f;
                }
                
                inputData.Move = math.normalize(new float2(dir.x, dir.z));
                
                return true;
            }

            return false;
        }
    }
}