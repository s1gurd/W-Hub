using GameFramework.Example.Components;
using GameFramework.Example.Utils;
using GameFramework.Example.Utils.LowLevel;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class PlayerMovementSystem : JobComponentSystem
    {
        [BurstCompile]
        private struct PlayerMovementJob : IJobForEach<PlayerInputData, ActorMovementData>
        {
            public void Execute(ref PlayerInputData input, ref ActorMovementData movement)
            {
                var inputVector = MathUtils.RotateVector(input.Move, 0 - input.CompensateAngle);
                var tempInput = inputVector;
                if (input.MinMagnitude == 0f)
                {
                    movement.Input = new float3(inputVector.x, 0f, inputVector.y);
                }
                else
                {
                    if ((math.lengthsq(inputVector) > 0) && (math.lengthsq(inputVector) < (input.MinMagnitude * input.MinMagnitude)))
                    {
                        tempInput = math.normalize(inputVector) * input.MinMagnitude;
                    }
                    movement.Input = new float3(tempInput.x, 0f, tempInput.y);
                }
            }
        }

        [BurstCompile]
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new PlayerMovementJob();
            return job.Schedule(this, inputDeps);
        }
    }
}