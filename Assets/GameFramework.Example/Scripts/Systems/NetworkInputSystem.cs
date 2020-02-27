using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace GameFramework.Example.Systems
{
    /// <summary>
    /// System for passing Network Inputs to Player Actor entities.
    /// In order to use, remove DisableAutoCreation attribute
    /// </summary>
    [DisableAutoCreation]
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class NetworkInputSystem : JobComponentSystem
    {
        private EntityCommandBufferSystem _barrier;

        private float2 _moveInput = default;
        private float2 _mouseInput = default;
        private float2 _lookInput = default;
        private NativeArray<float> _customInputs = new NativeArray<float>();


        protected override void OnCreate()
        {
            _customInputs = new NativeArray<float>(Constants.INPUT_BUFFER_CAPACITY, Allocator.Persistent);
            _barrier = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        }


        protected override void OnStartRunning()
        {
        }

        protected override void OnStopRunning()
        {
            _customInputs.Dispose();
        }


        [RequireComponentTag(typeof(ActionInputBuffer))]
        
        private struct PlayerInputJob : IJobForEachWithEntity<PlayerInputData, NetworkInputData>
        {
            public EntityCommandBuffer.Concurrent Ecb;

            [ReadOnly] public float2 MoveInput;
            [ReadOnly] public float2 MouseInput;
            [ReadOnly] public float2 LookInput;
            [ReadOnly] public NativeArray<float> CustomInputs;

            [NativeDisableParallelForRestriction] public BufferFromEntity<ActionInputBuffer> actionInputBuffer;

            private ActionInputBuffer _customInput;

            public void Execute(Entity entity, int index, ref PlayerInputData inputData, ref NetworkInputData network)
            {
                inputData.Move = MoveInput;
                inputData.Mouse = MouseInput;
                inputData.Look = LookInput;

                DynamicBuffer<ActionInputBuffer> inputBuffer = actionInputBuffer[entity];

                for (var i = 0; i < CustomInputs.Length; i++)
                {
                    _customInput.Value = CustomInputs[i];
                    inputBuffer[i] = _customInput;
                }
            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new PlayerInputJob
            {
                Ecb = _barrier.CreateCommandBuffer().ToConcurrent(),
                MoveInput = _moveInput,
                MouseInput = _mouseInput,
                LookInput = _lookInput,
                CustomInputs = _customInputs,
                actionInputBuffer = GetBufferFromEntity<ActionInputBuffer>(false)
            };
            inputDeps = job.Schedule(this, inputDeps);
            _barrier.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }
    }
}