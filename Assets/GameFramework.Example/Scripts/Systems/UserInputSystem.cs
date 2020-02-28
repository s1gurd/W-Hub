using System.Collections.Generic;
using System.Linq;
using GameFramework.Example.Common;
using GameFramework.Example.Components;
using GameFramework.Example.Utils.LowLevel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameFramework.Example.Systems
{
    [UpdateInGroup(typeof(FixedUpdateGroup))]
    public class UserInputSystem : JobComponentSystem
    {
        private EntityCommandBufferSystem _barrier;

        private InputAction _moveAction;
        private InputAction _mouseAction;
        private InputAction _lookAction;
        private List<InputAction> _customActions = new List<InputAction>();

        private float2 _moveInput;
        private float2 _mouseInput;
        private float2 _lookInput;
        private NativeArray<float> _customInputs = new NativeArray<float>();


        protected override void OnCreate()
        {
            _customInputs = new NativeArray<float>(Constants.INPUT_BUFFER_CAPACITY, Allocator.Persistent);
            _barrier = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        }


        protected override void OnStartRunning()
        {
            _moveAction = new InputAction("move", binding: "<Gamepad>/leftStick");
            _moveAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            _moveAction.performed += context => { _moveInput = context.ReadValue<Vector2>(); };
            _moveAction.canceled += context => { _moveInput = context.ReadValue<Vector2>(); };
            _moveAction.Enable();

            _mouseAction = new InputAction("mouse", binding: "<Mouse>/position");
            _mouseAction.performed += context => { _mouseInput = context.ReadValue<Vector2>(); };
            _mouseAction.canceled += context => { _mouseInput = context.ReadValue<Vector2>(); };
            _mouseAction.Enable();

            _lookAction = new InputAction("look", binding: "<Gamepad>/rightStick");
            _lookAction.AddCompositeBinding("Dpad")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
            //_lookAction.AddBinding(new InputBinding("<Pointer>/delta"));

            _lookAction.performed += context => { _lookInput = context.ReadValue<Vector2>(); };
            _lookAction.canceled += context => { _lookInput = context.ReadValue<Vector2>(); };
            _lookAction.Enable();

            //Here goes custom Actions for virtual keys 0..9
            for (var i = 0; i <= 9; i++)
            {
                var j = i;
                _customActions.Add(new InputAction($"CustomAction{j}", binding: $"<Keyboard>/{j}"));
                switch (j)
                {
                    case 0:
                        _customActions.Last().AddBinding(new InputBinding("<Mouse>/leftButton"));
                        break;
                    case 1:
                        _customActions.Last().AddBinding(new InputBinding("<Mouse>/rightButton"));
                        break;
                }

                _customActions.Last().performed += context => { _customInputs[j] = context.ReadValue<float>(); };
                _customActions.Last().canceled += context => { _customInputs[j] = context.ReadValue<float>(); };
                _customActions.Last().Enable();
            }
        }

        protected override void OnStopRunning()
        {
            _mouseAction.Disable();
            _lookAction.Disable();
            _moveAction.Disable();
            foreach (var c in _customActions)
            {
                c.Disable();
            }

            _customInputs.Dispose();
        }


        [RequireComponentTag(typeof(ActionInputBuffer))]
        private struct PlayerInputJob : IJobForEachWithEntity<PlayerInputData,UserInputData>
        {
            public EntityCommandBuffer.Concurrent Ecb;

            [ReadOnly] public float2 MoveInput;
            [ReadOnly] public float2 MouseInput;
            [ReadOnly] public float2 LookInput;
            [ReadOnly] public NativeArray<float> CustomInputs;

            [NativeDisableParallelForRestriction] public BufferFromEntity<ActionInputBuffer> actionInputBuffer;
            
            private ActionInputBuffer _customInput;

            public void Execute(Entity entity, int index, ref PlayerInputData inputData, ref UserInputData u)
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