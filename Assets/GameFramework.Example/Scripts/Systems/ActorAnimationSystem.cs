using System;
using GameFramework.Example.Components;
using Unity.Entities;
using UnityEngine;
using GameFramework.Example.Common;

namespace GameFramework.Example.Systems
{
    public class ActorAnimationSystem : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<ActorMovementAnimationData>(),
                ComponentType.ReadOnly<Animator>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_query).ForEach(
                (Entity entity, Animator animator, ref ActorMovementAnimationData animation, ref ActorMovementData movement) =>
                {
                    if (animator == null)
                    {
                        Debug.LogError("[MOVEMENT ANIMATION SYSTEM] No Animator found!");
                        return;
                    }
                    
                    var move = movement.MovementCache;
                    if (animation.AnimHash == 0 || animation.SpeedFactorHash == 0)
                    {
                        Debug.LogError("[MOVEMENT ANIMATION SYSTEM] Some hash(es) not found, check your Actor Movement Component Settings!");
                        return;
                    }
                    animator.SetBool(animation.AnimHash, Math.Abs(move.x) > Constants.MIN_MOVEMENT_THRESH || Math.Abs(move.y) > Constants.MIN_MOVEMENT_THRESH);
                    animator.SetFloat(animation.SpeedFactorHash,
                        animation.SpeedFactorMultiplier * movement.ExternalMultiplier * Math.Max(Math.Abs(move.x), Math.Abs(move.y)));
                });
        }
    }
}