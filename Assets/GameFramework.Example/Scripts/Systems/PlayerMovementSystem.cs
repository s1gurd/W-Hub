using GameFramework.Example.Components;
using GameFramework.Example.Utils;
using GameFramework.Example.Utils.LowLevel;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class PlayerMovementSystem : JobComponentSystem
    {
        
        private struct PlayerMovementJob : IJobForEach<PlayerInputData, ActorMovementData>
        {
            public void Execute(ref PlayerInputData input, ref ActorMovementData movement)
            {
                movement.Input = MathUtils.RotateVector(input.Move, 0 - input.CompensateAngle);
               
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new PlayerMovementJob();
            return job.Schedule(this, inputDeps);
        }
    }
}