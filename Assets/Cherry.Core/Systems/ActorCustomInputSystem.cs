using System;
using GameFramework.Example.Common;
using GameFramework.Example.Common.Interfaces;
using GameFramework.Example.Components;
using GameFramework.Example.Enums;
using GameFramework.Example.Utils.LowLevel;
using Unity.Entities;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    //[UpdateAfter(typeof(UserInputSystem))]
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
                    foreach (var b in mapping.bindingsDict)
                    {
                        if (Math.Abs(input.CustomInput[b.Key]) < Constants.INPUT_THRESH) continue;
                        b.Value.ForEach(a =>
                        {
                            if (a is IAimable) return;
                            a.Execute();
                            
                            if (mapping.inputSource != InputSource.UserInput) return;
                                
                            PostUpdateCommands.AddComponent(entity, new NotifyButtonActionExecutedData
                            {
                                ButtonIndex = b.Key
                            });


                        });
                    }
                });
        }
    }
}