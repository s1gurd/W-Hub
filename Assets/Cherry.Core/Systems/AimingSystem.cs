using System;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class AimingSystem : ComponentSystem
    {
        private EntityQuery _evaluateActionQuery;
        private EntityQuery _aimingQuery;
        private EntityQuery _projectileToDestroyQuery;


        protected override void OnCreate()
        {
            _aimingQuery = GetEntityQuery(
                ComponentType.ReadOnly<PlayerInputData>(),
                ComponentType.ReadOnly<AbilityPlayerInput>(),
                ComponentType.Exclude<DeadActorData>(),
                ComponentType.Exclude<DestructionPendingData>());

            _evaluateActionQuery = GetEntityQuery(
                ComponentType.ReadOnly<PlayerInputData>(),
                ComponentType.ReadOnly<AbilityPlayerInput>(),
                ComponentType.Exclude<DeadActorData>(),
                ComponentType.Exclude<DestructionPendingData>());

            _projectileToDestroyQuery = GetEntityQuery(
                ComponentType.ReadOnly<DestroyProjectileInPointData>(),
                ComponentType.ReadOnly<Transform>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_aimingQuery).ForEach(
                (Entity entity, AbilityPlayerInput mapping, ref PlayerInputData input) =>
                {
                    if (mapping.inputSource != InputSource.UserInput) return;

                    for (var i = 0; i <= input.CustomSticksInput.Length; i++)
                    {
                        var j = i;

                        var currentWeapons = mapping.customBindings
                            .Where(b => b.index == j)
                            .SelectMany(b => b.actions)
                            .Where(a => a is IAimable aimable && aimable.AimingAvailable)
                            .ToList();

                        if (!currentWeapons.Any()) continue;

                        var inputValue = (Vector2) input.CustomSticksInput[i];

                        if (inputValue == Vector2.zero)
                        {
                            currentWeapons.ForEach(a => ((IAimable) a).ResetAiming());
                            continue;
                        }

                        currentWeapons.ForEach(a => ((IAimable) a).EvaluateAim(inputValue));
                    }
                });

            Entities.With(_evaluateActionQuery).ForEach(
                (Entity entity, AbilityPlayerInput mapping, ref PlayerInputData input) =>
                {
                    foreach (var b in mapping.bindingsDict)
                    {
                        var aimables = b.Value.OfType<IAimable>().ToList();

                        if (Math.Abs(input.CustomInput[b.Key]) < Constants.INPUT_THRESH)
                        {
                            aimables.ForEach(a => a.ActionExecutionAllowed = true);
                            continue;
                        }
                        
                        aimables.ForEach(a =>
                        {
                            if (!a.ActionExecutionAllowed) return;
                            
                            (a as IActorAbility)?.Execute();

                            if (a.AimingProperties.evaluateActionOptions == EvaluateActionOptions.EvaluateOnce) a.ActionExecutionAllowed = false;
                            
                            if (mapping.inputSource != InputSource.UserInput) return;
                            
                            PostUpdateCommands.AddComponent(entity, new NotifyButtonActionExecutedData
                            {
                                ButtonIndex = b.Key
                            });
                        });
                    }
                });

            Entities.With(_projectileToDestroyQuery).ForEach(
                (Entity entity, Transform transform, ref DestroyProjectileInPointData point) =>
                {
                    if (Math.Abs(transform.position.x - point.Point.x) < 1 &&
                        Math.Abs(transform.position.z - point.Point.y) < 1)
                    {
                        transform.gameObject.DestroyWithEntity(entity);
                    }
                });
        }
    }
}