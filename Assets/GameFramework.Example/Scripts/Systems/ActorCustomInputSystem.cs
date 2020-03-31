using System;
using System.Collections.Generic;
using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Components.Interfaces;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    [UpdateAfter(typeof(UserInputSystem))]
    //[UpdateAfter(typeof(NetworkInputSystem))]
    //[UpdateAfter(typeof(AIInputSystem))]
    public class ActorCustomInputSystem : ComponentSystem
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = GetEntityQuery(
                ComponentType.ReadOnly<PlayerInputData>(),
                ComponentType.ReadOnly<AbilityPlayerInput>());
        }

        protected override void OnUpdate()
        {
            Entities.With(_query).ForEach(
                (Entity entity, AbilityPlayerInput mapping, ref PlayerInputData input) =>
                {
                    var buffer = input.CustomInput;

                    for (var i = 0; i < Constants.INPUT_BUFFER_CAPACITY; i++)
                    {
                        if (Math.Abs(buffer[i]) < Constants.INPUT_THRESH) continue;
                        var index = i;
                        mapping.customBindings.FindAll(b => b.index == index)
                            .ConvertAll(a => (List<MonoBehaviour>) a.actions).ForEach(actions =>
                                actions.ForEach(a => (a as IActorAbility)?.Execute()));
                    }
                });
        }
    }
}